using System;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace IntegrationTest
{
    public class WebsiteDemoTest
    {
        [Fact]
        public void LoginTest()
        {
            var serviceProviderEndpoint = ConfigurationManager.AppSettings["ServiceProviderEndpoint"];
            var username = ConfigurationManager.AppSettings["IdpUsername"];
            var password = ConfigurationManager.AppSettings["IdpPassword"];
            
            var options = new ChromeOptions();
            options.AcceptInsecureCertificates = true;
            options.AddArguments("headless");

            var driver = new ChromeDriver(options);
            
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                
                //Navigate to login page
                driver.Navigate().GoToUrl(serviceProviderEndpoint);                                
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("#bodyPanel>a:nth-of-type(1)"))).Click();
                
                //Log in
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("ctl00_ContentPlaceHolder1_UsernameTextbox")))
                    .SendKeys(username);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("ctl00_ContentPlaceHolder1_PasswordTextbox")))
                    .SendKeys(password);
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.CssSelector("input[type='submit']"))).Click();
                
                //Verify response
                wait.Until(d => d.FindElement(By.CssSelector("#userAttributes")));
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}