using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Markup;
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
        public string availability { get; set; }
        //TODO: Añadir más campos
        //public string summonerLevel { get; set; }
        //public string tagLine { get; set; }
    }

    public class SummonerFriendWIcon
    {
        public string gameName { get; set; }
        public string icon { get; set; }
        public string availability { get; set; }
        public BitmapImage iconImage { get; set; }

    }

    /// <summary>
    /// Lógica de interacción para Home.xaml
    /// </summary>
    public partial class Home : Window
    {

        LeagueClient leagueClient = new LeagueClient(credentials.cmd);
        public ObservableCollection<SummonerFriendWIcon> friendsList = new ObservableCollection<SummonerFriendWIcon>();



        public Home()
        {
            InitializeComponent();
            this.DataContext = this;

            SetOptionsTextToWhite();
        }

        public ObservableCollection<SummonerFriendWIcon> FriendsList
        {
            get { return friendsList; }
        }


        async public void GetData()
        {
            try
            {
                var data = await leagueClient.Request(requestMethod.GET, "/lol-summoner/v1/current-summoner");
                DeserializeSummonerInfo(data);
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

            DeserializeSummonerFriends(data);
        }

        private void DeserializeSummonerFriends(string summoner_friends)
        {
            var list = JsonConvert.DeserializeObject<List<SummonerFriend>>(summoner_friends);

            foreach (SummonerFriend friend in list)
            {
                SummonerFriendWIcon friendWIcon = new SummonerFriendWIcon();
                friendWIcon.gameName = friend.gameName;
                friendWIcon.icon = friend.icon;
                friendWIcon.availability = friend.availability;
                friendWIcon.iconImage = new BitmapImage(new Uri("https://ddragon.leagueoflegends.com/cdn/14.14.1/img/profileicon/" + friend.icon + ".png"));

                friendsList.Add(friendWIcon);
            }

            //Ordenar lista por availability
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lvFriendsList.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("availability", ListSortDirection.Ascending));




        }

        private void DeserializeSummonerInfo(string summoner_info)
        {
            Summoner current_summoner = JsonConvert.DeserializeObject<Summoner>(summoner_info);
            //Llamamos a esta URL para obtener el icono del summoner, sacándolo del ID del JSON
            imgIcon.ImageSource = new BitmapImage(new Uri("https://ddragon.leagueoflegends.com/cdn/14.14.1/img/profileicon/" + current_summoner.profileIconId + ".png"));
            tbSummName.Text = current_summoner.gameName + " #" + current_summoner.tagLine;
            System.Windows.Controls.Label lab = new System.Windows.Controls.Label();
            lab.Foreground = Brushes.White;


        }
    }
}
