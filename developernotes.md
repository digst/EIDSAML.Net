# Certificate generation

Certificates are issued using the following powershell

*   New-SelfSignedCertificate -DnsName "eidsaml-net.dk" -NotAfter "2030-01-01" -Provider "Microsoft Enhanced Cryptographic Provider v1.0"
*   New-SelfSignedCertificate -DnsName "eidsaml-demoidp.dk" -NotAfter "2030-01-01" -Provider "Microsoft Enhanced Cryptographic Provider v1.0"
*   New-SelfSignedCertificate -Subject "eidsaml test demoidp" -NotAfter "2030-01-01" -Provider "Microsoft Enhanced Cryptographic Provider v1.0"
*   New-SelfSignedCertificate -Subject "eidsaml test serviceprovider" -NotAfter "2030-01-01" -Provider "Microsoft Enhanced Cryptographic Provider v1.0"

# Creating a new EIDSAML .Net release

## Update documentation

Ensure documentation is updated with the changes, and that the release history is updated.

## Run integrationtests

Integration test must be performed on a test service provider. 

## Build the packages

* Run the BuildPackages.ps1, setting version to a proper version number, e.g 1.0.0 and assemblyVersion to a proper version number, e.g 1.0.0.0\. Use 1.0.0-alpha, 1.00-beta, etc. as version number to make a prerelease
* Verify the packages looks good and are ready to publish
* Ensure API key to digitaliseringsstyrelsen's nuget account is installed on your machine
* Push packages to NuGet by running BuildPackages.ps1 with the switch -pushPackages
* Add a tag in Git corresponding to the release

## Creating the new resource on digitaliser.dk

## Change the frontpage of the group
