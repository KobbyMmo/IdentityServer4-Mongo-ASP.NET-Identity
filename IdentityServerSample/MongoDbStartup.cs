using IdentityModel;
using IdentityServer4.Models;
using IdentityServerSample.Interface;
using IdentityServerSample.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerSample
{
    public static class MongoDbStartup
    {
        private static string _newRepositoryMsg = $"Mongo Repository created/populated! Please restart your website, so Mongo driver will be configured  to ignore Extra Elements.";

        /// <summary>
        /// Adds the support for MongoDb Persistance for all identityserver stored
        /// </summary>
        /// <remarks><![CDATA[
        /// It implements and used mongodb collections for:
        /// - Clients
        /// - IdentityResources
        /// - ApiResource
        /// ]]></remarks>
        public static void UseMongoDbForIdentityServer(this IApplicationBuilder app)
        {

            //Resolve Repository with ASP .NET Core DI help 
            var repository = (IRepository)app.ApplicationServices.GetService(typeof(IRepository));

            //Resolve ASP .NET Core Identity with DI help
            var userManager = (UserManager<ApplicationUser>)app.ApplicationServices.GetService(typeof(UserManager<ApplicationUser>));

            // --- Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing ---
            ConfigureMongoDriver2IgnoreExtraElements();

            var createdNewRepository = false;


            //  --Client
            if (!repository.CollectionExists<Client>())
            {
                foreach (var client in Config.GetClients())
                {
                    repository.Add(client);
                }
                createdNewRepository = true;
            }

            //  --IdentityResource
            if (!repository.CollectionExists<IdentityResource>())
            {
                foreach (var res in Config.GetIdentityResources())
                {
                    repository.Add(res);
                }
                createdNewRepository = true;
            }


            //  --ApiResource
            if (!repository.CollectionExists<ApiResource>())
            {
                foreach (var api in Config.GetApiResources())
                {
                    repository.Add(api);
                }
                createdNewRepository = true;
            }


            //Populate MongoDB with dummy users to enable test - e.g. Bob, Alice
            if (createdNewRepository == true)
            {
                AddSampleUsersToMongo(userManager);
            }


            // If it's a new Repository (database), need to restart the website to configure Mongo to ignore Extra Elements.
            if (createdNewRepository)
            {
                throw new Exception(_newRepositoryMsg);
            }

        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }

        /// <summary>
        /// Populate MongoDB with a List of Dummy users to enable tests - e.g. Alice, Bob
        ///   see Config.GetSampleUsers() for details.
        /// </summary>
        /// <param name="userManager"></param>
        private static void AddSampleUsersToMongo(Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> userManager)
        {
            var dummyUsers = Config.GetSampleUsers();

            foreach (var usrDummy in dummyUsers)
            {
                var userDummyEmail = usrDummy.Claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Email);

                if (userDummyEmail == null)
                {
                    throw new Exception("Could not locate user email from  claims!");
                }


                var user = new ApplicationUser()
                {
                    UserName = usrDummy.Username,
                    LockoutEnabled = false,
                    EmailConfirmed = true,
                    Email = userDummyEmail.Value,
                    NormalizedEmail = userDummyEmail.Value
                };



                foreach (var claim in usrDummy.Claims)
                {
                    user.AddClaim(claim);
                }
                var result = userManager.CreateAsync(user, usrDummy.Password);
                if (!result.Result.Succeeded)
                {
                    // If we got an error, Make sure to drop all collections from Mongo before trying again. Otherwise sample users will NOT be populated
                    var errorList = result.Result.Errors.ToArray();
                    throw new Exception($"Error Adding sample users to MongoDB! Make sure to drop all collections from Mongo before trying again!");
                }
            }
            return;
        }
    }
}
