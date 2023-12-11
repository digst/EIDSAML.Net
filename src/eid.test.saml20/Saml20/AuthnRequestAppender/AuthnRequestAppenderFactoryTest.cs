using eid.saml20.AuthnRequestAppender;
using NUnit.Framework;

namespace eid.test.saml20.AuthnRequestAppender
{
    [TestFixture]
    public class AuthnRequestAppenderFactoryTest
    {

        [Test]
        public void CanGetAppenderFromConfig()
        {
            //Arrange
            
            //This test assumes that app.config Federation element contains the following element
            //<AuthnRequestAppender type="eid.test.saml20.AuthnRequestAppender.AuthnRequestAppenderSample, eid.test.Saml20"/>

            //Act
            var appender = AuthnRequestAppenderFactory.GetAppender();
            //Assert
            Assert.NotNull(appender);
            Assert.IsInstanceOf<AuthnRequestAppenderSample>(appender);
        }
    }
}