using System;
using Xunit;

namespace Q42.ImageResizert.Test
{
    public class UrlHelperTests
    {
        [Theory]
        [InlineData(null, null, null, null, "/image/test")]
        [InlineData(10, null, null, null, "/image/test?w=10")]
        [InlineData(10, 20, null, null, "/image/test?w=10&h=20")]
        [InlineData(10, 20, false, null, "/image/test?w=10&h=20&cover=false")]
        [InlineData(10, 20, true, 50, "/image/test?w=10&h=20&cover=true&quality=50")]
        public void TestGetUrl(int? width, int? height, bool? cover, int? quality, string expected)
        {
            var settings = new ImageResizertSettings
            {
            };

            var imageResizert = new UrlHelper(settings);
            Assert.Equal(expected, imageResizert.GetUrlForImage("test", width, height, cover, quality).ToString());
        }

        [Theory]
        [InlineData("http://www.q42.com", "http://cdn.com", "http://cdn.com/image/test")]
        [InlineData("http://www.q42.com", null, "http://www.q42.com/image/test")]
        [InlineData(null, null, "/image/test")]
        public void TestGetUrlWithCDN(string hostname, string cdnName, string expected)
        {
            var settings = new ImageResizertSettings
            {
                BaseUrl = hostname,
                ImageCdn = cdnName
            };

            var imageResizert = new UrlHelper(settings);
            Assert.Equal(expected, imageResizert.GetUrlForImage("test").ToString());
        }
    }
}
