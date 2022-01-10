using Amazon;
using Aws.SecretManager.Console;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);
var config = builder.Build();

Console.WriteLine("Hello, Secret!");
Console.WriteLine("Dige o Secret Name");
Console.Write("Secret Name: ");
var secretName = Console.ReadLine();

var secret = new SecretManager(
    config["access_key"], 
    config["secret_key"],
    RegionEndpoint.SAEast1).Get(secretName ?? "");

Console.WriteLine(secret);