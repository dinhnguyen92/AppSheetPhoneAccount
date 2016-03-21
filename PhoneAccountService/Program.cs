using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace PhoneAccountService
{
    class Program
    {
        // Web service URLs and routes
        const string ENDPOINT_URL = "https://appsheettest1.azurewebsites.net/sample/";
        const string LIST_ROUTE = "list/";
        const string LIST_TOKEN_ROUTE = "list?token=";
        const string DETAIL_ROUTE = "detail/";

        // Number of accounts to display
        const int numRes = 5;

        static void Main(string[] args)
        {
            List<PhoneAcc> accounts = new List<PhoneAcc>(); // List for storing the final accounts to be displayed
            IDList idList = new IDList(); // Class for storing ID list retrieved from the web service

            // Retrieve the list of account IDs from the web service
            idList = getIDListFromWeb(null);
            Console.WriteLine("Retrieved ID list: " + idList.ToString());

            // Process the retrieved accounts and store valid accounts
            processPhoneAcc(idList, ref accounts, numRes);

            // While the retrieved list has a token for retrieving even more phone accounts
            // Keep retrieving and processing accounts
            while (!String.IsNullOrEmpty(idList.token))
            {
                // Retrieve the list of account IDs from the web service
                idList = getIDListFromWeb(idList.token);
                Console.WriteLine("Retrieved ID list: " + idList.ToString());

                // Process the retrieved accounts and store valid accounts
                processPhoneAcc(idList, ref accounts, numRes);
            }

            // Sort the collected accounts based on name
            AccNameCompare nameCompare = new AccNameCompare();
            accounts.Sort(nameCompare);

            // Print out the result
            Console.WriteLine();
            Console.WriteLine(accounts.Count + " results sorted by name: ");
            for (int i = 0; i < accounts.Count; i++)
            {
                Console.WriteLine(accounts[i].ToString());
            }


            // Wait for return key to pause the console
            Console.ReadLine();

        } // end of Main()


        /// <summary>
        /// Query the web service to retrieve the list of account IDs
        /// </summary>
        /// <param name="token">Token for retrieving ID list</param>
        /// <returns>The retrieved ID list</returns>
        private static IDList getIDListFromWeb(string token)
        {
            string json = null;

            // Retrieve a new list
            using (WebClient webClient = new WebClient())
            {
                // Retrieve and process the ID list
                try
                {
                    string url = ENDPOINT_URL;

                    // If there's no token
                    if (String.IsNullOrEmpty(token))
                    {
                        url = url + LIST_ROUTE;
                    }
                    else
                    {
                        url = url + LIST_TOKEN_ROUTE + token;
                    }

                    json = webClient.DownloadString(url);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            return JsonConvert.DeserializeObject<IDList>(json);
        }


        /// <summary>
        /// Iterate through the list of accounts to find the account with the largest age
        /// </summary>
        /// <param name="accounts">List of accounts</param>
        /// <returns>Index of account with largest age</returns>
        private static int getMaxAgeIndex(List<PhoneAcc> accounts)
        {
            if (accounts.Count == 0) return -1;

            int max = 0;

            for (int i = 0; i < accounts.Count; i++)
            {
                if (accounts[i].age > accounts[max].age)
                {
                    max = i;
                }
            }

            return max;
        }


        /// <summary>
        /// Process phone accounts retrieved from web service
        /// Store phone accounts with valid US phone numbers and youngest age to the account list
        /// </summary>
        /// <param name="idList">The list of account IDs retrieved from web service</param>
        /// <param name="accounts">The list of phone accounts to collect</param>
        /// <param name="numRes">The number of account to collect</param>
        private static void processPhoneAcc(IDList idList, ref List<PhoneAcc> accounts, int numRes)
        {
            // Iterate through all the retrieved IDs
            // Using each ID, retrieve the account from the web service
            // Add [numRes] accounts with valid number to the account list
            // If the number of accounts retrieved from web service is less than [numRes]
            // Add all the retrieved accounts with valid number
            int maxAgeIndex= 0; // track the ID of the account the max age among the collecte accounts
            for (int i = 0; i < idList.result.Count; i++)
            {
                string json = null;

                using (WebClient webClient = new WebClient())
                {
                    // Retrieve the account using ID
                    try
                    {
                        // URL: endpoint + detail route + retrieved id of account
                        json = webClient.DownloadString(ENDPOINT_URL + DETAIL_ROUTE + idList.result[i]);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                PhoneAcc acc = JsonConvert.DeserializeObject<PhoneAcc>(json);
                Console.WriteLine("Retrieved account: " + acc.ToString());

                if (acc.hasValidNumber())
                {
                    // If not enough results have been collected
                    if (accounts.Count < numRes)
                    {
                        Console.WriteLine("Adding account with ID: " + acc.id);
                        accounts.Add(acc);

                        // Update the index of the account with max age
                        if (acc.age >= accounts[maxAgeIndex].age)
                        {
                            // The index of the new acc is count - 1
                            maxAgeIndex = accounts.Count - 1;
                        }
                    }
                    // If enough results have been collected
                    else
                    {
                        // Compare the retrieved acc to the collected acc with max age
                        if (acc.age < accounts[maxAgeIndex].age)
                        {
                            Console.WriteLine("Replacing account " + accounts[maxAgeIndex].id + " with account " + acc.id);
                            // Replace the acc with max age with the retrieved acc
                            accounts[maxAgeIndex] = acc;

                            // Update the index of the account with max age
                            maxAgeIndex = getMaxAgeIndex(accounts);
                        }
                    }
                } 
            } 
        }
    } 
}
