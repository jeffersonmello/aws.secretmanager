using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Aws.SecretManager.Console;

public class SecretManager
{
    private string _access_key;
    private string _secrete_key;
    
    private RegionEndpoint _regionEndpoint;


    public SecretManager(string accessKey, string secreteKey, RegionEndpoint regionEndpoint)
    {
        _access_key = accessKey;
        _secrete_key = secreteKey;
        _regionEndpoint = regionEndpoint;
    }

    public string Get(string secretName)
    {
        var secret = "";

        var memoryStream = new MemoryStream();

        var client = new AmazonSecretsManagerClient(_access_key, _secrete_key, RegionEndpoint.SAEast1);

        var request = new GetSecretValueRequest();
        request.SecretId = secretName;
        request.VersionStage = "AWSCURRENT"; // VersionStage defaults to AWSCURRENT if unspecified.

        GetSecretValueResponse response = null;
        
        try
        {
            response = Task.Run(async () => await client.GetSecretValueAsync(request)).Result;
        }
        catch (ResourceNotFoundException)
        {
            System.Console.WriteLine("The requested secret " + secretName + " was not found");
        }
        catch (InvalidRequestException e)
        {
            System.Console.WriteLine("The request was invalid due to: " + e.Message);
        }
        catch (InvalidParameterException e)
        {
            System.Console.WriteLine("The request had invalid params: " + e.Message);
        }
        
        // Decrypts secret using the associated KMS key.
        // Depending on whether the secret is a string or binary, one of these fields will be populated.
        if (response.SecretString != null)
        {
            secret = response.SecretString;
        }
        else
        {
            memoryStream = response.SecretBinary;
            var reader = new StreamReader(memoryStream);
            var decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            secret = decodedBinarySecret;
        }

        return secret;
    }
}