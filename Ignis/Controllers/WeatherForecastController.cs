using Microsoft.AspNetCore.Mvc;

namespace Ignis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
//// We assume GdPicture has been correctly installed and unlocked.
//GdPictureImaging oGdPictureImaging = new GdPictureImaging();
//// Loading the image in direct access mode.
//int ImageID = oGdPictureImaging.CreateGdPictureImageFromFile("image.JPG", false, true);
//if (oGdPictureImaging.GetStat() == GdPictureStatus.OK)
//{
//    System.Text.StringBuilder sb = new System.Text.StringBuilder();
//    int exifTagsCount = oGdPictureImaging.TagCount(ImageID);
//    if (exifTagsCount != 0)
//    {
//        // Handling EXIF tags.
//        sb.Append("EXIF tags:\n-----------\n");
//        for (int i = 1; i <= exifTagsCount; i++)
//        {
//            string tagName = oGdPictureImaging.TagGetName(ImageID, i);
//            string tagFormattedValue = oGdPictureImaging.TagGetValueString(ImageID, i);
//            sb.Append(tagName + ": " + tagFormattedValue + "\n");
//        }
//    }
//    oGdPictureImaging.ReleaseGdPictureImage(ImageID);
//    MessageBox.Show(sb.ToString(), "Metadata Example", MessageBoxButtons.OK, MessageBoxIcon.Information);
//}
//else
//{
//    MessageBox.Show("The image can't be loaded. Status: " + oGdPictureImaging.GetStat().ToString(), "Metadata Example", MessageBoxButtons.OK, MessageBoxIcon.Error);
//}
//oGdPictureImaging.Dispose();