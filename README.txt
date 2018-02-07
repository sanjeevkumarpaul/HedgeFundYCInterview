public const string TenantName = "nycscab2c.onmicrosoft.com";
        public const string TenantId = "b67c229d-b81c-4a4a-8aa0-4e05f0a8c196";
        public const string ClientId = "ce94c404-3481-4d00-8bfc-a4a12c08bb50";  //Also called ApplicationID
        public const string ClientSecret = "oit2B4de31YIautZ1HfZ+NuFemByetsnwcfT2lt42d8=";
        public const string AuthString = "https://login.microsoftonline.com/" + TenantId;
        public const string ResourceUrl = "https://graph.windows.net";
        public const string AuthResourceUrl = "https://graph.windows.net/me";



internal class AuthenticationHelper
    {
        public static ActiveDirectoryClient GetActiveDirectoryClientAsApplication()
        {
            Uri servicePointUri = new Uri(Constants.ResourceUrl);
            Uri serviceRoot = new Uri(servicePointUri, Constants.TenantId);
            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot, async () => await AcquireTokenAsyncForApplication());
            return activeDirectoryClient;
        }
        
        public static async Task<string> AcquireTokenAsyncForApplication()
        {
            AuthenticationContext authenticationContext = new AuthenticationContext(Constants.AuthString, false);
            // Config for OAuth client credentials 
            ClientCredential clientCred = new ClientCredential(Constants.ClientId, Constants.ClientSecret);
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(Constants.ResourceUrl, clientCred);
            string token = authenticationResult.AccessToken;
            System.Diagnostics.Debug.WriteLine("Non Authenticated Access: " + token);
            return token;
        }

        // // // MY TESTING WITH USER CREDENTIAL // // //
        public static ActiveDirectoryClient GetActiveDirectoryClientAsUser(string userName, string password)
        {
            Uri servicePointUri = new Uri(Constants.ResourceUrl);
            Uri serviceRoot = new Uri(servicePointUri, Constants.TenantId);
            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot, async () => await AcquireTokenWithUserCredentials(userName, password));
           
            return activeDirectoryClient;
        }

        public static async Task<string> AcquireTokenWithUserCredentials(string userName, string password)
        {
            var authContext = new AuthenticationContext(Constants.AuthString, false );
            var userCreds = new UserCredential(userName, password);
            System.Diagnostics.Debug.WriteLine("Trying to Authenticate and fetch Token...");

            ClientCredential clientCred = new ClientCredential(Constants.ClientId, Constants.ClientSecret);
            var result = await authContext.AcquireTokenAsync(Constants.AuthResourceUrl, clientCred);
            result = await authContext.AcquireTokenAsync(Constants.ResourceUrl, Constants.ClientId, userCreds);

            string token = result.AccessToken; 
            System.Diagnostics.Debug.WriteLine("Authenticated Access Token : " + token);
            return token;
            
        }
    }
    
    
    #region Setup Active Directory Client
            ActiveDirectoryClient activeDirectoryClient;
            try
            {
                activeDirectoryClient = AuthenticationHelper.GetActiveDirectoryClientAsApplication();
            }
            catch (AuthenticationException ex)
            {
                if (ex.InnerException != null)
                {
                    txtOutput.Text += $"Error detail: {ex.InnerException.Message}";
                }
                return;
            }
            #endregion
            
            
try
            {
                
                //Group by user
                IGroup groups = activeDirectoryClient.Groups.Where(d => d.ObjectId == groupId).ExecuteSingleAsync().Result;
                Group group = (Group)groups;
                IGroupFetcher groupFetcher = group;
                var members = groupFetcher.Members.ExecuteAsync().Result;
                
                do
               {
                   //Azure Async always comes in pages.
                   if (members.CurrentPage.Count != 0)
                   {
                        if (member is user)
                        {
                            User user = member as user;

                        }
                   }
                   
                   members = members.GetNextPageAsync().Result;
                } while (members != null);
                //end
                
                // -- or -- we need to segrigate.
            
                //IUser users = activeDirectoryClient.Users.Where(d => d.ObjectId == userId).ExecuteSingleAsync().Result;
                IUser users = activeDirectoryClient.Users.Where(d => d.UserPrincipalName.Equals(userId, StringComparison.CurrentCultureIgnoreCase ) ).ExecuteSingleAsync().Result;
                User user = (User)users;

               

               //other way

                txtDetails.Text = $"User name: {user.DisplayName}" + NewLine;
                txtDetails.Text += $"Mail address: {user.Mail}" + NewLine;
                txtDetails.Text += $"Mobile phone: {user.Mobile}" + NewLine;
                txtDetails.Text += $"UPN: {user.UserPrincipalName}"+ NewLine;
                txtDetails.Text += $"Firs tName(Given Name): {user.GivenName}" + NewLine;
                txtDetails.Text += $"Last Name(Sur Name): {user.Surname}" + NewLine;
                txtDetails.Text += $"ObjectId: {user.ObjectId}" + NewLine + NewLine;
                txtDetails.Text += "Sign In Names - " + NewLine;
                foreach(var name in user.SignInNames)
                {
                    txtDetails.Text += $"{name.Type} : {name.Value}. {NewLine}";
                }
                txtDetails.Text += NewLine + NewLine;
                txtDetails.Text += $"Mail Nick: {user.MailNickname}";

            }
            catch
            {
                txtOutput.Text += "Oops, did you forget to select a user first?" + NewLine;
            }

            var context = activeDirectoryClient.Users.Context;


//all users

            
    
    
    
