using System.Net.Http;
using System.Text.RegularExpressions;
using System;
using System.Text;

namespace WetterdatenCacheMVC.Models;

public class WeatherdataFetcher
{
    private readonly string _url = "https://opendata.dwd.de/weather/text_forecasts/txt/";

    public List<WeatherDataset> WeatherDatasets { get; set; } = new();

    public IOrderedEnumerable<WeatherDataset> WeatherDatasetsSorted()
    {
        return WeatherDatasets.OrderByDescending( x => x.Date ).ThenByDescending( y => y.Time );
    }

    public async Task FetchWeatherDataAsync()
    {
        var httpClient = new HttpClient();

        var regexLinks = new Regex( @"href=""([^""]*)""" );

        var html = await httpClient.GetStringAsync( _url );

        MatchCollection matches = regexLinks.Matches( html );

        List<string> results = new();

        #region
        //foreach ( Match match in matches )
        //{
        //    var result = match.Value.Replace( "href=\"" , "" ).Replace( "\"" , "" );

        // //result = result.Replace( "\"" , "" );

        //    if ( !result.Contains( ".pdf" ) )
        //        results.Add( result );
        //}
        #endregion

        Parallel.ForEach( matches , match =>
        {
            var result = match.Value.Replace( "href=\"" , "" ).Replace( "\"" , "" );

            if ( !result.Contains( ".pdf" ) )
                results.Add( result );
        } );

        results.RemoveAt( 0 );

        List<string> filecontents = new();

        #region
        //foreach ( string result in results )
        //{
        //    HttpResponseMessage response = await httpClient.GetAsync( _url + result );
        //    response.EnsureSuccessStatusCode();

        // byte [] byteArray = await response.Content.ReadAsByteArrayAsync();

        // var content = Encoding.UTF7.GetString( byteArray );

        //    filecontents.Add( content );
        //}
        #endregion

        await Parallel.ForEachAsync( results , async ( result , cancellationToken ) =>
        {
            HttpResponseMessage response = await httpClient.GetAsync( _url + result );
            response.EnsureSuccessStatusCode();

            byte [] byteArray = await response.Content.ReadAsByteArrayAsync();

            var content = Encoding.UTF7.GetString( byteArray );

            filecontents.Add( content );
        } );

        #region
        //foreach ( string item in filecontents )
        //{
        //    var regexOrt = new Regex( @"(?<=\b(Wetterberatung|Wetterberatungszentrale)\s)[\w-]+" );
        //    var regexDate = new Regex( @"\b\d{2}\.\d{2}\.\d{4}\b" );
        //    var regexTime = new Regex( @"\b\d{2}:\d{2}\b" );

        // Match ortMatch = regexOrt.Match( item ); Match dateMatch = regexDate.Match( item ); Match
        // timeMatch = regexTime.Match( item ); string time = DateTime.Now.ToString();

        //    if ( ortMatch.Success && dateMatch.Success && timeMatch.Success )
        //    {
        //        WeatherDatasets.Add( new WeatherDataset() { Location = ortMatch.Value , Date = dateMatch.Value , Time = timeMatch.Value , Timestamp = time } );
        //    }
        //}
        #endregion

        Parallel.ForEach( filecontents , ( item , cancellationToken ) =>
        {
            var regexOrt = new Regex( @"(?<=\b(Wetterberatung|Wetterberatungszentrale)\s)[\w-]+" );
            var regexDate = new Regex( @"\b\d{2}\.\d{2}\.\d{4}\b" );
            var regexTime = new Regex( @"\b\d{2}:\d{2}\b" );

            Match ortMatch = regexOrt.Match( item );
            Match dateMatch = regexDate.Match( item );
            Match timeMatch = regexTime.Match( item );

            string time = DateTime.Now.ToString();

            if ( ortMatch.Success && dateMatch.Success && timeMatch.Success )
            {
                WeatherDatasets.Add( new WeatherDataset() { Location = ortMatch.Value , Date = dateMatch.Value , Time = timeMatch.Value , Timestamp = time } );
            }
        } );
    }
}