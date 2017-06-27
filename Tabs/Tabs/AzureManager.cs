using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Tabs
{
    public class AzureManager
    {
		private static AzureManager instance;
		private MobileServiceClient client;
        private IMobileServiceTable<celebrityModel> celebrityModelTable;

		private AzureManager()
		{
			this.client = new MobileServiceClient("http://celebrityrecogniser.azurewebsites.net/");
            this.celebrityModelTable = this.client.GetTable<celebrityModel>();
		}

		public MobileServiceClient AzureClient
		{
			get { return client; }
		}

		public static AzureManager AzureManagerInstance
		{
			get
			{
				if (instance == null)
				{
					instance = new AzureManager();
				}

				return instance;
			}
		}
		public async Task<List<celebrityModel>> GetCelebrityInformation()
		{
            return await this.celebrityModelTable.ToListAsync();
		}
        public async Task PostCelebrityInformation(celebrityModel celebrityModel)
		{
            await this.celebrityModelTable.InsertAsync(celebrityModel);
		}
        public async Task UpdateCelebrityInformation(celebrityModel celebrityModel)
		{
            await this.celebrityModelTable.UpdateAsync(celebrityModel);
		}
        public async Task DeleteCelebrityInformation(celebrityModel celebrityModel)
		{
            await this.celebrityModelTable.DeleteAsync(celebrityModel);
		}
    }
}
