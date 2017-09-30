using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;

namespace TestChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();

            logger.Debug("Start");

            try
            {
                string channelName = "kaveori";
                string accessToken = "*******";
                ConnectionCredentials credentials = new ConnectionCredentials(channelName, accessToken);
                TwitchClient client = new TwitchClient(credentials, channelName, logging: true);

                client.OnLog += Client_OnLog;
                client.OnConnectionError += Client_OnConnectionError;
                client.OnDisconnected += Client_OnDisconnected;

                client.Connect();

                client.OnMessageReceived += new EventHandler<OnMessageReceivedArgs>(Twitch_OnMessageReceived);
            } catch (Exception ex)
            {
                string innerMsg = ex.InnerException?.Message ?? "";
                logger.Error($"Twitch GetViewersAsync: {ex.Message}, {innerMsg}");
            }
            

        }

        private void Client_OnDisconnected(object sender, OnDisconnectedArgs e)
        {
            logger.Debug($"Disconnect: {ToJson(e)}");
        }

        private void Twitch_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            logger.Debug(ToJson(e));
        }

        private string ToJson(object value)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            return JsonConvert.SerializeObject(value, Formatting.Indented, settings);
        }

        private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
        {
            logger.Error($"Connection Error: {e.Error}");
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            logger.Debug($"Debug Log: {e.Data}");
        }
    }
}
