# PowerServer-Authentication-Example

The Authentication integration app demonstrates how to integrate multiple authentication policies into the server side of PowerServer. It is developed with Appeon [PowerBuilder](https://www.appeon.com/products/powerbuilder) and deployed with Appeon [PowerServer 2021](https://www.appeon.com/products/powerserver). 

### Sample Project Structure

The project is structured as follows.

```
|—— PowerServer-Authentication-Example Repository 
	|—— Client 		PowerBuilder client project integrated with the authentication
	|—— Server		PowerServer project integrated with the authentication
```

### Setting Up the Project

Download this PowerServer demo application, and then:

1. Open the PowerBuilder project in PowerBuilder 2021.

2. Download the database file <b>pbdemo2021_for_sqlanywhere.zip</b> from [PowerBuilder-Project-Example-Database](https://github.com/Appeon/PowerBuilder-Project-Example-Database) and restore it. 

3. Configure the ODBC data source.

4. Open the PowerServer project: ps_authentication, switch to the Web APIs tab and set Web API URL and it has to be using HTTPS.

5. Click the Auto Import button in the License settings to import your License.

6. Open Database Configuration. Set the database to the ODBC source you set in step #3.

7. Register a test project in AWS Cognito, Azure AD, or Azure B2C based on actual requirements. (The routing of the callback address based on the authorization code mode must be consistent with that configured in the middleware on the server, for example, the callback address of AWS is https://localhost:4000/aws/callback.)

8. Open the CloudSetting.ini file in the Client folder, change the host of TokenEndpoint and UserEndpoint to the Web API URL you set in step #4, change the AuthorizeUrl of AWS and Azure to your personal account configuration, and change Username and Password according to your account information.

9. Open the deployed ServerAPIs project and fill in all the sensitive data between the angle brackets `<>` in the Authentication.json file, for example:

   ep:

   ```
   "AWS": {
   	       	"Region": "<your region>", (Fill in the Region you used when registering in AWS between the angle brackets.)
   			...
   	}
   ```

10. Build & Deploy the PowerServer project.

11. Run the ServerAPIs project.

12. Run the PowerServer project.

### PowerServer Deployment

The source code includes a PowerServer project. If you want to deploy it using PowerServer, you need to go to the project object, switch to the Web APIs tab, and import your own license and then you can directly deploy it. (If you current PowerBuilder login account has PowerServer license, you can choose Auto Import, otherwise, you can choose Import from file.)
