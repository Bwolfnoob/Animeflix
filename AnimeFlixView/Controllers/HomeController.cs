using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AnimeFlixView.ViewModel;
using Google.Apis.YouTube.v3;
using Google.Apis.Services;

namespace AnimeFlixView.Controllers
{
    public class HomeController : Controller
    {
        private static IList<MainTelaViewModel> JaCarregados;
        public IActionResult Index(string id = "UCggu7ZqCPm962udvkv-pMRQ")
        {
            @ViewBag.IdCanal = id;
            if (JaCarregados != null)
            {
                var main = JaCarregados.FirstOrDefault(c => c.CanalId.Equals(id));
                if (main == null)
                {
                    MainTelaViewModel mainTela = CarregarMainTela(id);
                    return View(mainTela);
                }
                else
                {
                    return View(main);
                }
            }
            else
            {
                JaCarregados = new List<MainTelaViewModel>();
                MainTelaViewModel mainTela = CarregarMainTela(id);
                return View(mainTela);
            }
        }

        private MainTelaViewModel CarregarMainTela(string id)
        {
            
            MainTelaViewModel mainTela = new MainTelaViewModel();
            mainTela.Itens = new List<ItemViewModel>();

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "AIzaSyBVf3-QzJn93Zddi7XfCNe4YIXmgCsKTiA",
                ApplicationName = this.GetType().ToString()
            });

            var Canal = youtubeService.Channels.List("contentDetails,snippet,TopicDetails,brandingSettings");
            Canal.Id = id;
            var CanalResponse = Canal.Execute().Items.First();
            mainTela.Capa = CanalResponse.BrandingSettings.Image.BannerImageUrl;
            mainTela.TituloCanal = CanalResponse.Snippet.Title;
            mainTela.Descricao = CanalResponse.Snippet.Description.Length > 300 ? CanalResponse.Snippet.Description.Substring(0, 300) : CanalResponse.Snippet.Description;


            var channelsRequest = youtubeService.Playlists.List("contentDetails,snippet");
            channelsRequest.MaxResults = 50;
            channelsRequest.ChannelId = id;

            var channelsListResponse = channelsRequest.Execute();

            foreach (var channel in channelsListResponse.Items)
            {
                //  var uploadsListId = channelsRequest.Id;// channel.ContentDetails.RelatedPlaylists.Uploads;
                ItemViewModel itemViewModel = new ItemViewModel(channel.Snippet.Title);
                itemViewModel.Videos = new List<VideoViewModel>();

                var nextPageToken = "";
                while (nextPageToken != null)
                {
                    var playlistItemsListRequest = youtubeService.PlaylistItems.List("contentDetails,snippet");
                    playlistItemsListRequest.PlaylistId = channel.Id;
                    playlistItemsListRequest.MaxResults = 50;
                    playlistItemsListRequest.PageToken = nextPageToken;
                    var playlistItemsListResponse = playlistItemsListRequest.Execute();
                    foreach (var item in playlistItemsListResponse.Items)
                    {
                        try
                        {
                            itemViewModel.Videos.Add(new VideoViewModel()
                            {
                                Titulo = item.Snippet.Title,
                                Descricao = item.Snippet.Description.Length > 150 ? item.Snippet.Description.Substring(0, 50) : item.Snippet.Description,
                                ImagemVideo = item.Snippet.Thumbnails.High.Url,
                                LinkVideo = "https://www.youtube.com/embed/" + item.Snippet.ResourceId.VideoId,
                                PlaylistId = item.Snippet.PlaylistId
                            });
                        }
                        catch (Exception)
                        {
                        }

                    }

                    nextPageToken = playlistItemsListResponse.NextPageToken;
                }
                mainTela.Itens.Add(itemViewModel);
            }
            mainTela.CanalId = id;
            JaCarregados.Add(mainTela);
            return mainTela;
        }

        public IActionResult AbrirVideo(string linkVideo , string id = "UCggu7ZqCPm962udvkv-pMRQ")
        {
            @ViewBag.IdCanal = id;
            return View(new AbrirVideoViewModel() { LinkVideo=linkVideo});
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
