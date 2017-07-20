/*Copyright 2017 Eric Yang*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Diagnostics;
using System.IO.Ports;
using System.Xml;
using System.Media;
using System.Net;

namespace Voice_Bot
{
    public partial class Form1 : Form
    {
        Boolean italk = false;
        SpeechSynthesizer s = new SpeechSynthesizer();
        Boolean wake = true;
        Boolean pockey = true;
        Boolean isitonce = false;
        SoundPlayer player = new SoundPlayer();
        DateTime bfr = new DateTime();
        WebClient w = new WebClient();

        SerialPort port = new SerialPort("COM3", 9600, Parity.None,
            8, StopBits.One);

        

        String temp;
        String condition;

        Choices list = new Choices();
        public Form1()
        {


            SpeechRecognitionEngine rec = new SpeechRecognitionEngine();

            list.Add(new String[] { "hello", "how are you",
                "how are you doing", "what time is it",
                "what is today", "what day is today",
                "what is the date",
                "open google", "wake up", "sleep",
                "restart", "update", "light on",
                "light off", "oh really",
                /*"open bing",*/ "open minecraft", "close minecraft",
                "close voice bot", "whats the weather like",
                "whats the temperature",
                "pink fluffy encrypted unicorns",
                "thank you", "bot",
                "please play super secret music",
                "minimize", "maximize", "tell me a joke",
                "unminimize", "unmaximize", "play", "pause",
                "spotify", "next", "last"});

            Grammar gr = new Grammar(new GrammarBuilder(list));

            WindowsMicrophoneMuteLibrary.WindowsMicMute micMute = new WindowsMicrophoneMuteLibrary.WindowsMicMute();

            try
            {

                rec.RequestRecognizerUpdate();
                rec.LoadGrammar(gr);
                rec.SpeechRecognized += rec_SpeachRecognized;
                rec.SetInputToDefaultAudioDevice();
                rec.RecognizeAsync(RecognizeMode.Multiple);

            }
            catch { return; }

            s.SelectVoiceByHints(VoiceGender.Female);

            InitializeComponent();
        }

        public String GetWeather(String input)
        {
            String query = String.Format("https://query.yahooapis.com/v1/public/yql?q=select * from weather.forecast where woeid in (select woeid from geo.places(1) where text='allen, tx')&format=xml&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys");
            XmlDocument wData = new XmlDocument();
            try
            {
                wData.Load(query);
            }
            catch
            {
                MessageBox.Show("No Internet Connection");
                return "No internet";
            }
            XmlNamespaceManager manager = new XmlNamespaceManager(wData.NameTable);
            manager.AddNamespace("yweather", "http://xml.weather.yahoo.com/ns/rss/1.0");

            XmlNode channel = wData.SelectSingleNode("query").SelectSingleNode("results").SelectSingleNode("channel");
            XmlNodeList nodes = wData.SelectNodes("query/results/channel");
            try
            {
                int rawtemp = int.Parse(channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["temp"].Value);
                temp = (rawtemp - 32) * 5/9 + "" ;
                condition = channel.SelectSingleNode("item").SelectSingleNode("yweather:condition", manager).Attributes["text"].Value;
                if (input == "temp")
                {
                    return temp;
                }
               
                
                if (input == "cond")
                {
                    return condition;
                }
            }
            catch
            {
                return "Error Reciving data";
            }
            return "error";
        }

        public void restart()
        {
            if (pockey == true)
            {
                Process.Start("Voice Bot.exe");
                Environment.Exit(0);
            }
        }

        public static void killProg(String s)
        {
            System.Diagnostics.Process[] procs = null;

            try
            {
                procs = Process.GetProcessesByName(s);
                Process prog = procs[0];

                if (!prog.HasExited)
                {
                    prog.Kill();
                }
            }

            finally
            {
                if(procs != null)
                {
                    foreach (Process p in procs)
                    {
                        p.Dispose();
                    }
                }
            }
            procs = null;
        }

        public void say(String h)
        {
            if (pockey == true)
            {
                if(isitonce == false)
                {
                    bfr = DateTime.Now;
                }
                DateTime afr = new DateTime();
                afr = DateTime.Now;
                int safr = afr.Second;
                int sbfr = bfr.Second;
                if(safr - sbfr >= 7)
                {
                    italk = false;
                    Console.WriteLine(safr + "and" + sbfr);
                }
                else if (safr - sbfr <= 6 && safr - sbfr >= 0)
                {
                    isitonce = true;
                    Console.WriteLine(safr + "and" + sbfr);
                }
                else
                {
                    safr += 60;
                    isitonce = true;
                }
                bfr = DateTime.Now;
                WindowsMicrophoneMuteLibrary.WindowsMicMute micMute = new WindowsMicrophoneMuteLibrary.WindowsMicMute();
                micMute.MuteMic();
                pockey = false;
                s.Speak(h);
                textBox2.AppendText(h + "\n");
                micMute.UnMuteMic();
               // if (isitonce == false)
                //{
                    
                    
                    
                //}
                //secb = bfr.Second;
                pockey = true;
                /*seca = afr.Second;
                if (seca - secb >= 10)
                {
                    italk = false;

                }
                else if (seca - secb <= 9)
                {
                    isitonce = true;
                }
                else
                {
                    seca += 60;

                }*/
                
            }
        }

        String[] greetings = new String[3] { "Hi", "Hello", "How are you" };

        public String greetings_action()
        {

            Random r = new Random();
            return greetings[r.Next(3)];

        }

        //Commands

        private void rec_SpeachRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            String r = e.Result.Text;

            if (r == "bot")
            {
                italk = true;
                SystemSounds.Beep.Play();
            }


            if (italk == true)
            {
                if (r == "wake up" && pockey == true)
                {
                    wake = true;
                    say("the bot has woken up");
                    label3.Text = "State: Awake";
                }
                if (r == "sleep" && pockey == true)
                {
                    wake = false;
                    say("the bot is now resting");
                    label3.Text = "State: Sleep";
                }


                if (wake == true)
                {



                    if (r == "hello") //When you say...
                    {
                        say(greetings_action());  //What it says...
                    }
                    
                    if(r == "tell me a joke")
                    {
                        say(w.DownloadString("http://api.yomomma.info/").Replace("\"", "").Replace(":", "").Replace("joke", ""));
                    }

                    if (r == "please play super secret music")
                    {
                        say("Playing");
                        player.SoundLocation = "C:\\Users\\Bob\\Music\\videoplayback.wav";
                        player.Play();
                    }

                    if (r == "minimize")
                    {
                        this.WindowState = FormWindowState.Minimized;
                    }

                    if (r == "unminimize" || r == "unmaximize")
                    {
                        this.WindowState = FormWindowState.Normal;
                    }

                    if (r == "maximize")
                    {
                        this.WindowState = FormWindowState.Maximized;
                    }

                    if (r == "spotify")
                    {
                        Process.Start(@"C:\Users\Bob\AppData\Roaming\Spotify\Spotify.exe");
                        restart();
                    }

                    if (r == "play" || r == "pause")
                    {
                        SendKeys.Send(" ");
                    }

                    if (r == "next")
                    {
                        SendKeys.Send("^{RIGHT}");
                    }

                    if (r == "last")
                    {
                        SendKeys.Send("^{LEFT}");
                    }

                    if (r == "thank you") //When you say...
                    {
                        say("your welcome");  //What it says...
                    }

                    if (r == "pink fluffy encrypted unicorns")
                    {
                        say("Hello creator. Thank you for creating me");
                    }

                    if (r == "restart" || r == "update")
                    {
                        say("restarting");
                        restart();
                    }

                    if (r == "whats the weather like") //When you say...
                    {
                        say("the sky is " + GetWeather("cond") + ",");  //What it says...
                    }

                    if (r == "whats the temperature") //When you say...
                    {
                        say("It is " + GetWeather("temp") + " degrees");  //What it says...
                    }

                    if (r == "how are you" || r == "how are you doing") //When you say...
                    {
                        say("Great, and you?");  //What it says...
                    }

                    if (r == "close voice bot") //When you say...
                    {
                        Environment.Exit(0);  //What it says...
                    }

                    if (r == "what time is it") //When you say...
                    {
                        say(DateTime.Now.ToString("h:mm tt"));  //What it says...
                    }

                    if (r == "what day is today" ||
                            r == "what is today" ||
                            r == "what is the date") //When you say...
                    {
                        say(DateTime.Now.ToString("M/d/yyyy"));  //What it says...
                    }

                    if (r == "open google") //When you say...
                    {
                        Process.Start("http://google.com");
                        restart();//What it says...
                    }

                    /*if (r == "open bing") //When you say...
                    {
                        Process.Start("http://bing.com");  //What it says...
                    }
                    */
                    if (r == "light on")
                    {
                        try
                        {
                            port.Open();
                            port.WriteLine("A");
                            port.Close();
                        }
                        catch { say("you don't have an arduino"); }
                    }
                    if (r == "light off")
                    {
                        try
                        {
                            port.Open();
                            port.WriteLine("B");
                            port.Close();
                        }
                        catch
                        {
                            say("you don't have an arduino");
                        }
                    }
                    if (r == "oh really")
                    {
                        say("Yes really");
                    }
                    if (r == "open minecraft")
                    {
                        try
                        {
                            Process.Start(@"C:\Program Files (x86)\Minecraft\MinecraftLauncher.exe");
                            restart();
                        }
                        catch
                        {
                            say("you either don't have minecraft installed, or you don't have an x64 system");
                        }
                    }

                    if (r == "close minecraft")
                    {
                        try
                        {
                            killProg("javaw");
                        }
                        catch
                        {
                            say("minecraft isn't running");
                        }
                    }
                }
                textBox1.AppendText(r + "\n");

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
