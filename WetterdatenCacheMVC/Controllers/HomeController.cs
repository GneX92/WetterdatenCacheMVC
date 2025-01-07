using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WetterdatenCacheMVC.Models;

namespace WetterdatenCacheMVC.Controllers
{
    public class HomeController : Controller
    {
        private IMemoryCache _memoryCache;
        private WeatherdataFetcher _fetcher;

        public HomeController( WeatherdataFetcher fetcher , IMemoryCache memoryCache )
        {
            _fetcher = fetcher;
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            var cacheEntry = _memoryCache.GetOrCreate( "WeatherData" , entry =>
            {
                _fetcher.FetchWeatherDataAsync().Wait();
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours( 1 );
                return _fetcher.WeatherDatasetsSorted();
            } );

            return View( cacheEntry );
        }

        public IActionResult Update()
        {
            _memoryCache.Remove( "WeatherData" );

            return RedirectToAction( "Index" );
        }

        [ResponseCache( Duration = 0 , Location = ResponseCacheLocation.None , NoStore = true )]
        public IActionResult Error()
        {
            return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
        }
    }
}