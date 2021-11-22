using KZOMNAV.Controllers.Users;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace KZOMNAV
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string Token { get; set; }
        public static string GrupoName { get; set; }
        public static bool Humanizacao { get; set; }
        public static string Nav { get; set; }
        public static List<string> Contas { get; set; }
        public static bool IsGrupo { get; set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 4)
            {
                IsGrupo = true;
                Token = e.Args[0];
                GrupoName = e.Args[1];
                Humanizacao = e.Args[2] == "true";
                Nav = e.Args[3];
                var check = UserController.CheckToken(Token);
                bool checkToken = check.Status == 1 ? true : false; //Checar o token
                if (!checkToken)
                {
                    MessageBox.Show(check.Erro);
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else
            {
                if (e.Args.Length > 4)
                {
                    IsGrupo = false;
                    Token = e.Args[0];
                    GrupoName = e.Args[1];
                    Humanizacao = e.Args[2] == "true";
                    Nav = e.Args[3];
                    Contas = new List<string>();
                    for (int i = 4; i < e.Args.Length; i++)
                    {
                        Contas.Add(e.Args[i]);
                    }
                    var check = UserController.CheckToken(Token);
                    bool checkToken = check.Status == 1 ? true : false; //Checar o token
                    if (!checkToken)
                    {
                        MessageBox.Show(check.Erro);
                        System.Windows.Application.Current.Shutdown();
                    }
                }
                else
                {
                    MessageBox.Show($"Erro argumentos: {e.Args}");
                    System.Windows.Application.Current.Shutdown();
                }
            }
        }
    }
}
