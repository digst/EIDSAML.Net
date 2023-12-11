The [EIDSAML.Net](https://github.com/digst/eIDASSAML.Net) was built from the [OIOSAML.Net](https://github.com/digst/OIOSAML.Net) to enable Danish Service Providers integration with the Danish Eidas Gateway and avoid library conflicts with the OIOSAML.Net Library.

See content and changes of releases in [release notes](RELEASE_NOTES.md).

# Getting started with EIDSAML.Net

This is the codebase that the EIDSAML.Net components are built from.

## Resource links

*   [Code repository](hhttps://github.com/digst/eIDASSAML.Net)

## Repository content

*   **build**: Contains script to create and publish NuGet packages
*   **certificates**: Certificates used for getting the demo sample up and running
*   **setup**: Setup scripts used for getting demo sample up and running
*   **src**: source code for the EIDSAML.Net framework
*   **developer notes.html**: Information relevant for developers of EIDSAML.Net
*   **readme.html**: This file

## Getting started

The source code contains everything you need to get a demonstration environment up and running, federating with your own local Identity Provider, as well as directly against Danish Eidas Gateway.

For a quick setup, you must do the following:

*   Run the script 'setup\setup_prerequisites.ps1' from an elevated powershell. This installs all required certificates and performs sslcert bindings to be able to host local websites using https
*   Open the solution 'eid.saml20.sln' in Visual Studio 2019 (Elevated mode) and build it (if you get errors on external dependencies, ensure nuget packages are being restored)
*   Set the projects 'IdentityProviderDemo' and 'WebsiteDemo' as startup projects by right-clicking solution, select 'properties', selecting 'Multiple start projects'
*   For the web projects, you must manually set the 'Start URL' that IIS express uses. You do this by:
    *   right click project 'IdentityProviderDemo', select 'properties', select the tab 'Web', alter the 'Start Action' to the radio button 'Start URL', specifying 'https://eidsaml-demoidp.dk:30001'
    *   right click project 'WebsiteDemo', select 'properties', select the tab 'Web', alter the 'Start Action' to the radio button 'Start URL', specifying 'https://eidsaml-net.dk:30002'
*   Run the solution which should start IIS express for the two websites

This should start two browser windows, one for the demo idp ('IdentityProviderDemo'), and one for the service provider ('WebsiteDemo').  

On the service provider you should now be able to log in using either the demo idp or Danish Eidas Gateway, by selecting the identity provider in the list of identity providers:  

* If you choose the eID gateway, you must have select the Test Country EU in the [Test Danish Eidas Gateway](https://eidasconnector.test.eid.digst.dk/).
* If you choose the local demo idp, you log in with a username/password with one of the users listed in the web.config file for the demo idp under the 'demoIdp' section.
