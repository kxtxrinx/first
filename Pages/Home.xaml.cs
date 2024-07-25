using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.Remoting.Messaging;
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

    public class SummonerFriend
    {
        public string gameName { get; set; }
        public string icon { get; set; }
        //TODO: Añadir más campos
        //public string summonerLevel { get; set; }
        //public string tagLine { get; set; }
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
            SetOptionsTextToWhite();


        }


        async public void GetData()
        {
            try
            {
                var data = await leagueClient.Request(requestMethod.GET, "/lol-summoner/v1/current-summoner");
                deserealize_summoner_info(data);
            }
            catch (System.InvalidOperationException)
            {
                tbSummName.Text = "Error al cargar los datos.\n¿Está el cliente corriendo con una sesión iniciada?";
            }
        }

        private void ReloadData(object sender, RoutedEventArgs e)
        {
            GetData();
            GetFriendList();
        }

        private void SetOptionsTextToWhite()
        {

            foreach (var component in panelOptions.Children)
            {
                if (component is System.Windows.Controls.Label)
                {
                    System.Windows.Controls.Label label = (System.Windows.Controls.Label)component;
                    label.Foreground = Brushes.White;
                }

            }
        }


        private async void GetFriendList()
        {
            var data = await leagueClient.Request(requestMethod.GET, "/lol-chat/v1/friends");
            lblFriendUser.Text = data;
        }

        private void deserealize_summoner_info(string summoner_info)
        {
            Summoner current_summoner = JsonConvert.DeserializeObject<Summoner>(summoner_info);
            //Llamamos a esta URL para obtener el icono del summoner, sacándolo del ID del JSON
            imgIcon.ImageSource = new BitmapImage(new Uri("https://ddragon.leagueoflegends.com/cdn/11.15.1/img/profileicon/" + current_summoner.profileIconId + ".png"));
            tbSummName.Text = current_summoner.gameName + " #" + current_summoner.tagLine;
            System.Windows.Controls.Label lab = new System.Windows.Controls.Label();
            lab.Foreground = Brushes.White;


        }
    }
}
