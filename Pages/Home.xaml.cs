using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json;
using PoniLCU;
using static PoniLCU.LeagueClient;

namespace RiotTesting.Pages
{

    public class Summoner
    {
        public string displayName { get; set; }
        public string gameName { get; set; }
        public string profileIconId { get; set; }
        public string summonerLevel { get; set; }
        public string tagLine { get; set; }
    }
    /// <summary>
    /// Lógica de interacción para Home.xaml
    /// </summary>
    public partial class Home : Window
    {

        LeagueClient leagueClient = new LeagueClient(credentials.cmd);

        public Home()
        {
            InitializeComponent();


        }



        async public void getData()
        {
            var data = await leagueClient.Request(requestMethod.GET, "/lol-summoner/v1/current-summoner");
            deserealize_summoner_info(data);
        }

        private void ReloadData(object sender, RoutedEventArgs e)
        {
            getData();
            
        }


        private void deserealize_summoner_info(string summoner_info)
        {
            Summoner current_summoner = JsonConvert.DeserializeObject<Summoner>(summoner_info);
            //Llamamos a esta URL para obtener el icono del summoner, sacándolo del ID del JSON
            imgIcon.Source = new BitmapImage(new Uri("https://ddragon.leagueoflegends.com/cdn/11.15.1/img/profileicon/" + current_summoner.profileIconId + ".png"));
            lblSummName.Content = current_summoner.displayName + " #" + current_summoner.tagLine;
        }
    }
}
