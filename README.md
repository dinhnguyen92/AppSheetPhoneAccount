# AppSheetPhoneAccount
Simple program to retrieve phone accounts from AppSheet database and display 5 accounts with the youngest age and valid US phone number

*** Project requirements ***

Make a C# program (or use any other language) that outputs the 5 youngest users with valid US telephone numbers sorted by name


*** AppSheet Web Service ***

The web service for accessing the phone account database is provided by AppSheet.
Service Endpoint: https://appsheettest1.azurewebsites.net/sample/

Web Service Methods:

1. list: 
This method will return an array of up to 10 user IDs.  
If there are more than 10 results the response will also contain a token that can be used to retrieve the next set of results.  
This optional token can be passed as a query string parameter
Eg:  https://appsheettest1.azurewebsites.net/sample/list or https://appsheettest1.azurewebsites.net/sample/list?token=b32b3

2. detail/{user id}:
This method will returns the full details for a given user
Eg:  https://appsheettest1.azurewebsites.net/sample/detail/21


*** Project's main classes ***

1. Program.cs: the program class containing the Main method
2. PhoneAcc.cs: class for representing phone account objects retrieved from database using the detail/{user id} method
3. IDList.cs: class for representing the list of account IDs retrieved from database using the list method
4. AccNameCompare.cs: IComparator class used for comparing accounts based on name


*** Algorithm Overview ***

There are two main approaches to solving the given problem: 

_ Retrieve all phone accounts with valid phone numbers from database, sort the accounts based on age, and return the first 5 accounts.

_ Use the list method to retrieve accounts in batches. For each batch, keep the accounts the meet the requirements, discard the rest.

The first approach has two main disadvantages: 

_ Memory intensive: all of the accounts retrieved from the database must be stored

_ Time-consuming: sorting the entire database of phone accounts can be extremely slow

Since this program may be used by AppSheet to provide phone account search services to mobile devices, slow run time is unacceptable.
Hence the second approach is chosen.
In the second approach, at any time at most 5 valid accounts with the youngest ages are kept. 
For every batch of accounts retrieved, if there's am accounts with smaller age than the "oldest" account on hand, swap the accounts.
Keep retrieving and processing new account batches as long as the retrieved ID list has a token for accessing more accounts.

In the program, the URLs are defined as constants in the Program class. 
In case the method URLs of the web service change, this will allow the URLs in the program to be changed quickly.
Similarly, the number of accounts to return is defined as a constant in the Program class.
This allows the number of accounts to return to be changed easily if necessary.

The main disadvantage of the second approach:
The program must keep track of the account with the highest age among the 5 accounts to be returned.
Each time the account collection changes, the program must iterate through the collection to find the account with the highest age.
If the number of accounts to collect is large, this can slow down the program significantly.
However, since the accounts returned are most probably for display on a mobile device with relattively small screen,
only a small number of accounts must be returned.


*** Third Party Plug-in ***

Apart from the .NET libraries used, the json.NET plug-in published by Newtonsoft is used for processing JSON objects.


*** Automated Testing Suggestion ***

Currently, PhoneAcc objects are instantiated using JSON objects retrieved from the database.
For automated testing, a simple program can be written to generate dummy PhoneAcc objects without having to access the database.
The program can be written to generate edge cases (for eg. negative age, null phone number, etc.).
The dummy PhoneAcc objects can then be fed to the program's processPhoneAcc() method to test the method's correctness.
Naturally the processPhoneAcc() must also be modified to work without using the IDList or access to the database.

Another way to test the app is to set up a dummy database. This approach is slower since a database must be set up.
However, this will allow the whole app to be tested unaltered. 
