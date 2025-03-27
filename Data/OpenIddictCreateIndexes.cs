// .                    @@             _____ _______ _    _ _____ _____ ____
//          @       @@@@@             / ____|__   __| |  | |  __ \_   _/ __ \
//         @@@  @@@@                 | (___    | |  | |  | | |  | || || |  | |
//         @@@@@@@@@  @    @          \___ \   | |  | |  | | |  | || || |  | |
//        @@@@       @@@@@@@          ____) |  | |  | |__| | |__| || || |__| |
//    @@@@@             @@@          |_____/   |_|   \____/|_____/_____\____/
//     @@@      @@@@@@@  @@                   __                    ___
//      @@   @@@@    @@  @@                  /  \ |\ | |    | |\ | |__
//      @@  @@@     @@   @   @               \__/ | \| |___ | | \| |___
//    @@@@  @@@   @@@@   @@@@
//     @@@@  @@@@@  @@@@@@@         https://github.com/UnlostWorld/StudioOnline
//       @@@
//        @@@@@@@@@@@@@@                This software is licensed under the
//            @@@@  @                  GNU AFFERO GENERAL PUBLIC LICENSE v3

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;

public static class OpenIddictFunctions
{
	public static async Task GenerateIndexes(IServiceProvider provider)
	{
		var context = provider.GetRequiredService<IOpenIddictMongoDbContext>();
		var options = provider.GetRequiredService<IOptionsMonitor<OpenIddictMongoDbOptions>>().CurrentValue;
		var database = await context.GetDatabaseAsync(CancellationToken.None);

		var applications = database.GetCollection<OpenIddictMongoDbApplication>(options.ApplicationsCollectionName);

		await applications.Indexes.CreateManyAsync(
		[
			new CreateIndexModel<OpenIddictMongoDbApplication>(
				Builders<OpenIddictMongoDbApplication>.IndexKeys.Ascending(application => application.ClientId),
				new CreateIndexOptions
				{
					Unique = true,
				}),

			new CreateIndexModel<OpenIddictMongoDbApplication>(
				Builders<OpenIddictMongoDbApplication>.IndexKeys.Ascending(application => application.PostLogoutRedirectUris),
				new CreateIndexOptions
				{
					Background = true,
				}),

			new CreateIndexModel<OpenIddictMongoDbApplication>(
				Builders<OpenIddictMongoDbApplication>.IndexKeys.Ascending(application => application.RedirectUris),
				new CreateIndexOptions
				{
					Background = true,
				})
		]);

		var authorizations = database.GetCollection<OpenIddictMongoDbAuthorization>(options.AuthorizationsCollectionName);

		await authorizations.Indexes.CreateOneAsync(
			new CreateIndexModel<OpenIddictMongoDbAuthorization>(
				Builders<OpenIddictMongoDbAuthorization>.IndexKeys
					.Ascending(authorization => authorization.ApplicationId)
					.Ascending(authorization => authorization.Scopes)
					.Ascending(authorization => authorization.Status)
					.Ascending(authorization => authorization.Subject)
					.Ascending(authorization => authorization.Type),
				new CreateIndexOptions
				{
					Background = true,
				}));

		var scopes = database.GetCollection<OpenIddictMongoDbScope>(options.ScopesCollectionName);

		await scopes.Indexes.CreateOneAsync(new CreateIndexModel<OpenIddictMongoDbScope>(
			Builders<OpenIddictMongoDbScope>.IndexKeys.Ascending(scope => scope.Name),
			new CreateIndexOptions
			{
				Unique = true,
			}));

		var tokens = database.GetCollection<OpenIddictMongoDbToken>(options.TokensCollectionName);

		await tokens.Indexes.CreateManyAsync(
		[
			new CreateIndexModel<OpenIddictMongoDbToken>(
				Builders<OpenIddictMongoDbToken>.IndexKeys.Ascending(token => token.ReferenceId),
				new CreateIndexOptions<OpenIddictMongoDbToken>
				{
					// Note: partial filter expressions are not supported on Azure Cosmos DB.
					// As a workaround, the expression and the unique constraint can be removed.
					PartialFilterExpression = Builders<OpenIddictMongoDbToken>.Filter.Exists(token => token.ReferenceId),
					Unique = true,
				}),

			new CreateIndexModel<OpenIddictMongoDbToken>(
				Builders<OpenIddictMongoDbToken>.IndexKeys.Ascending(token => token.AuthorizationId),
				new CreateIndexOptions<OpenIddictMongoDbToken>()
				{
					PartialFilterExpression =
						Builders<OpenIddictMongoDbToken>.Filter.Exists(token => token.AuthorizationId),
				}),

			new CreateIndexModel<OpenIddictMongoDbToken>(
				Builders<OpenIddictMongoDbToken>.IndexKeys
					.Ascending(token => token.ApplicationId)
					.Ascending(token => token.Status)
					.Ascending(token => token.Subject)
					.Ascending(token => token.Type),
				new CreateIndexOptions
				{
					Background = true,
				})
		]);
	}
}