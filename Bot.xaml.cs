#region System
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Windows.Media;
#endregion
#region Selenium
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Interactions;
#endregion
#region Classes
using KZOMNAV.Controllers;
using KZOMNAV.Models;
using KZOMNAV.Models.Instagrams;
using KZOMNAV.Models.Blocks;
using Block = KZOMNAV.Models.Blocks.Block;
using Retorno = KZOMNAV.Models.Retornos.Retorno_Two;
#endregion

namespace KZOMNAV
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Bot : Window
    {

        #region Variaveis
        private Models.Grupos.Grupo Grupo { get; set; }
        private Models.Globals.Global Global { get; set; }
        private List<Models.Instagrams.Instagram> Contas { get; set; }
        private List<InstaGNI> InstagramGNI { get; set; }
        private string Email { get; set; }
        private string Password { get; set; }
        private List<Block> Bloqueios { get; set; }
        private List<string> Challenges { get; set; }
        private List<string> Incorrects { get; set; }
        private IWebDriver Driver { get; set; }
        private IJavaScriptExecutor js { get; set; }
        private WebDriverWait wait { get; set; }
        private double Total = 0;
        private double Saldo = 0;
        private double Meta = 0;
        private DateTime inicio = DateTime.Now;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private int Index = 0;
        private string NormalCollor = "#07bb9f";
        private string BlockCollor = "#f5c300";
        private string ChallengeCollor = "#f03c22";
        private string IncorrectCollor = "#fe8495";
        #endregion

        public Bot()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(TimerTick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            tx_curtir.Text = "0";
            tx_meta.Text = "0/0";
            tx_saldo.Text = "$0,0";
            tx_seguir.Text = "0";
            tx_tconta.Text = "0";
            tx_total.Text = "0";
            tx_username.Text = "@";
            InitializeDados();
        }

        #region KZOM
        /// <summary>
        /// Realizar login no Kzom através do navegador
        /// </summary>
        /// <returns>Bool</returns>
        private async Task<bool> LoginKzom()
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();
                if (File.Exists($@"{dir}\Config\kzom.txt"))
                {
                    await Task.Delay(548);
                    string[] linhas = File.ReadAllLines($@"{dir}\Config\kzom.txt");
                    if (linhas.Length == 2)
                    {
                        Email = linhas[0];
                        Password = linhas[1];
                        if (Driver != null)
                        {
                            Driver.Navigate().GoToUrl("https://kzom.com.br/login/");
                            wait.Until(d => d.FindElement(By.Name("email")));
                            try
                            {
                                Driver.FindElement(By.Name("email")).SendKeys(Email);
                                await Task.Delay(1000);
                                Driver.FindElement(By.Name("senha")).SendKeys(Password);
                                await Task.Delay(1000);
                                Driver.FindElement(By.TagName("button")).Click();
                                try
                                {
                                    wait.Until(d => d.FindElement(By.XPath("//div[text()='Total Pontos']")));
                                    return true;
                                }
                                catch
                                {
                                    return false;
                                }
                            }
                            catch
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            } catch
            {
                return false;
            }
        }
        /// <summary>
        /// Confirmar uma tarefa
        /// </summary>
        /// <returns>Bool</returns>
        private async Task<Retorno> ConfirmarAcao(string username)
        {
            Retorno Retorno = new Retorno
            { Status = 0, Response = "" };
            try
            {
                try
                {
                    //wait.Until(d => d.FindElement(By.Id("confirma")));
                    var waits = new List<Models.Waits.Wait>
                    {
                        new Models.Waits.Wait {VALUE = "confirma", TYPE = Models.Waits.WaitTypes.TYPE_ID}
                    };
                    var r = await Models.Waits.Waits.Wait(waits, 5, Driver);
                    if (r == 0)
                    {
                        var element = Driver.FindElement(By.Id("confirma"));
                        var actions = new Actions(Driver);
                        actions.MoveToElement(element);
                        actions.Perform();
                        await Task.Delay(300);
                        element.Click();
                        await Task.Delay(300);
                        Retorno.Status = 1;
                        Retorno.Response = "Tarefa Confirmada";
                        return Retorno;
                    }
                    else
                    {
                        Retorno.Status = -1;
                        Retorno.Response = "Erro ao confirmar a tarefa";
                        return Retorno;
                    }
                }
                catch
                {
                    Retorno.Status = -1;
                    Retorno.Response = "Erro ao confirmar a tarefa";
                    return Retorno;
                }
            }
            catch
            {
                Retorno.Status = -3;
                Retorno.Response = "Erro ao confirmar a tarefa";
                return Retorno;
            }
        }
        /// <summary>
        /// Pular uma tarefa
        /// </summary>
        /// <returns>Bool</returns>
        private async Task<Retorno> PularAcao(string username)
        {
            Retorno Retorno = new Retorno
            { Status = 0, Response = "" };
            try
            {
                try
                {
                    var waits = new List<Models.Waits.Wait>
                    {
                        new Models.Waits.Wait {VALUE = "pular", TYPE = Models.Waits.WaitTypes.TYPE_ID}
                    };
                    var r = await Models.Waits.Waits.Wait(waits, 5, Driver);
                    if (r == 0)
                    {
                        var element = Driver.FindElement(By.Id("pular"));
                        var actions = new Actions(Driver);
                        actions.MoveToElement(element);
                        actions.Perform();
                        await Task.Delay(300);
                        element.Click();
                        await Task.Delay(300);
                        Retorno.Status = 1;
                        Retorno.Response = "Tarefa Pulada";
                        return Retorno;
                    }
                    else
                    {
                        Retorno.Status = -1;
                        Retorno.Response = "Erro ao pular a tarefa";
                        return Retorno;
                    }
                }
                catch
                {
                    Retorno.Status = -1;
                    Retorno.Response = "Erro ao pular a tarefa";
                    return Retorno;
                }
            }
            catch
            {

                Retorno.Status = -3;
                Retorno.Response = "Erro ao pular a tarefa";
                return Retorno;
            }
        }
        /// <summary>
        /// Checar se a conta está cadastrada no kzom
        /// </summary>
        /// <returns>Bool</returns>
        private async Task<Retorno> CheckAccountKzom(string username)
        {
            Retorno ret = new Retorno
            {
                Response = "",
                Status = 0
            };
            try
            {
                Driver.Navigate().GoToUrl("https://kzom.com.br/painel/add.php");
                await Task.Delay(349);
                var waits = new List<Models.Waits.Wait>
                {
                    new Models.Waits.Wait {VALUE = "c1", TYPE = Models.Waits.WaitTypes.TYPE_ID },
                    new Models.Waits.Wait {VALUE = "email", TYPE = Models.Waits.WaitTypes.TYPE_NAME }
                };
                var r = await Models.Waits.Waits.Wait(waits, 10, Driver);
                if (r == 0)
                {
                    waits.Clear();
                    waits.Add(new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = $"//div[text()='{username}']" });
                    r = await Models.Waits.Waits.Wait(waits, 3, Driver);
                    if (r == 0)
                    {
                        ret.Status = 1;
                        ret.Response = "Conta encontrada";
                        return ret;
                    } else
                    {
                        ret.Status = 0;
                        ret.Response = "Conta não encontrada";
                        return ret;
                    }
                } else
                {
                    var login = await LoginKzom();
                    if (login)
                    {
                        return await CheckAccountKzom(username);
                    } else
                    {
                        ret.Status = -1;
                        ret.Response = "Não foi possivel logar no kzom";
                        return ret;
                    }
                }
            } catch
            {
                ret.Status = -1;
                ret.Response = "Erro ao verificar a conta";
            }
            return ret;
        }
        /// <summary>
        /// Pegar uma tarefa do kzom
        /// </summary>
        /// <returns>Bool</returns>
        private async Task<Retorno> GetTaskKzom(string usernmae)
        {
            Retorno ret = new Retorno
            {
                Response = "",
                Status = 0
            };
            try
            {
                if (Driver.Url.IndexOf(usernmae.ToLower()) < 0)
                {
                    Driver.Navigate().GoToUrl($"https://kzom.com.br/painel/seguir.php?user={usernmae.ToLower()}");
                }
                await Task.Delay(589);
                var waits = new List<Models.Waits.Wait>
                {
                    new Models.Waits.Wait {VALUE = "//button[text()='Iniciar']", TYPE = Models.Waits.WaitTypes.TYPE_XPATH },
                    new Models.Waits.Wait {VALUE = "email", TYPE = Models.Waits.WaitTypes.TYPE_NAME }
                };
                await Task.Delay(597);
                var r = await Models.Waits.Waits.Wait(waits, 10, Driver);
                if (r == 0)
                {
                    var element = Driver.FindElement(By.XPath("//button[text()='Iniciar']"));
                    try
                    {
                        var wait1 = new WebDriverWait(Driver, TimeSpan.FromSeconds(20));
                        wait.Until(d => d.FindElement(By.XPath("//button[text()='Exibir Link']")));
                        await Task.Delay(179);
                        Driver.FindElement(By.XPath("//button[text()='Exibir Link']")).Click();
                        await Task.Delay(1879);
                        if (Driver.WindowHandles.Count > 1)
                        {
                            Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                            await Task.Delay(794);
                            if (Driver.Url.IndexOf("/p/") > -1)
                            {
                                ret.Status = 2;
                                ret.Response = "Curtir";
                                return ret;
                            }
                             else
                            {
                                ret.Status = 1;
                                ret.Response = "Seguir";
                                return ret;
                            }
                        } else
                        {
                            ret.Status = 0;
                            ret.Response = "Não foi possivel encontrar tarefa";
                            return ret;
                        }
                    } catch
                    {
                        element.Click();
                        await Task.Delay(497);
                    }
                } else
                {
                    var login = await LoginKzom();
                    if (login)
                    {
                        return await GetTaskKzom(usernmae);
                    } else
                    {
                        ret.Status = -1;
                        ret.Response = "Erro ao buscar tarefa";
                        return ret;
                    }
                }
            } catch
            {
                ret.Status = -1;
                ret.Response = "Erro ao buscar tarefa";
                return ret;
            }
            return ret;
        }
        #endregion

        #region Instagram
        /// <summary>
        /// Realizar login no instagram
        /// </summary>
        /// <param name="Insta">Conta para logar</param>
        /// <returns></returns>
        private async Task<Retorno> Login(Models.Instagrams.Instagram Insta)
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                Driver.Navigate().GoToUrl("https://www.instagram.com/accounts/logout");
                wait.Until(d => d.FindElement(By.Name("username")));
                await Task.Delay(1249);
                Driver.Navigate().GoToUrl("https://www.instagram.com/");
                try
                {
                    wait.Until(d => d.FindElement(By.Name("username")));
                }
                catch
                {
                    Driver.Navigate().GoToUrl("https://www.instagram.com/");
                    wait.Until(d => d.FindElement(By.Name("username")));
                }
                await Task.Delay(597);
                try
                {
                    Driver.FindElement(By.XPath("//button[text()='Aceitar tudo']")).Click();
                }
                catch
                {
                    try
                    {
                        Driver.FindElement(By.XPath("//button[text()='Accept All']")).Click();
                    }
                    catch
                    { }
                }
                Driver.FindElement(By.Name("username")).SendKeys(Insta.Username);
                await Task.Delay(100);
                Driver.FindElement(By.Name("password")).SendKeys(Insta.Password);
                await Task.Delay(500);
                try
                {
                    js.ExecuteScript("document.evaluate(\"//div[text()='Entrar']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                }
                catch
                {
                    js.ExecuteScript("document.evaluate(\"//div[text()='Log In']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                }
                var waits = new List<Models.Waits.Wait>
                {
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_CSS, VALUE = "img[data-testid=user-avatar]" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_CSS, VALUE = "label[for=choice_0]" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_CSS, VALUE = "p[data-testid=login-error-message]"}
                };
                var r = await Models.Waits.Waits.Wait(waits, 15, Driver);
                switch (r)
                {
                    case 0:
                        try
                        {
                            await Task.Delay(345);
                            Driver.FindElement(By.XPath("//button[text()='Agora não']")).Click();
                            await Task.Delay(2645);
                        }
                        catch { }
                        try
                        {
                            await Task.Delay(345);
                            Driver.FindElement(By.XPath("//button[text()='Not Now']")).Click();
                            await Task.Delay(2645);
                        }
                        catch { }
                        try
                        {
                            await Task.Delay(345);
                            Driver.FindElement(By.XPath("//button[text()='Agora não']")).Click();
                            await Task.Delay(2645);
                        }
                        catch { }
                        try
                        {
                            await Task.Delay(345);
                            Driver.FindElement(By.XPath("//button[text()='Not Now']")).Click();
                            await Task.Delay(2645);
                        }
                        catch { }
                        wait.Until(d => d.FindElement(By.CssSelector("img[data-testid=user-avatar]")));
                        try
                        {
                            await Task.Delay(1249);
                            IWebElement ativity = null;
                            try
                            {
                                ativity = Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Feed de atividades']//ancestor::a[1]"));
                            }
                            catch
                            {
                                ativity = Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Activity Feed']//ancestor::a[1]"));
                            }
                            try
                            {
                                ativity.Click();
                                await Task.Delay(3178);
                            }
                            catch { }
                            try
                            {
                                Driver.FindElement(By.CssSelector("body")).Click();
                                await Task.Delay(597);
                            }
                            catch { }
                            Driver.FindElement(By.XPath("//nav//div//div//div//div//div//div//span//img//ancestor::span[1]")).Click();
                            //js.ExecuteScript("document.evaluate(\"//nav//div//div//div//div//div//div//span//img//ancestor::span[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            await Task.Delay(1247);
                            try
                            {
                                js.ExecuteScript("document.evaluate(\"//div[text()='Configurações']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                await Task.Delay(1248);
                                wait.Until(d => d.FindElement(By.XPath("//a[text()='Atividade de login']")));
                                await Task.Delay(248);
                                Driver.FindElement(By.XPath("//a[text()='Atividade de login']")).Click();
                                await Task.Delay(248);
                                wait.Until(d => d.FindElement(By.XPath("//h2[text()='Atividade de login']")));
                                await Task.Delay(467);
                                try
                                {
                                    js.ExecuteScript("document.evaluate(\"//div[text()='Fui eu']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                    await Task.Delay(897);
                                    js.ExecuteScript("document.evaluate(\"//button[text()='Confirmar']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                    await Task.Delay(1297);
                                    js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Página inicial']//ancestor::a[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                }
                                catch
                                {
                                    js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Página inicial']//ancestor::a[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                }
                            }
                            catch
                            {
                                js.ExecuteScript("document.evaluate(\"//div[text()='Settings']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                await Task.Delay(1248);
                                wait.Until(d => d.FindElement(By.XPath("//a[text()='Login Activity']")));
                                await Task.Delay(248);
                                Driver.FindElement(By.XPath("//a[text()='Login Activity']")).Click();
                                await Task.Delay(248);
                                wait.Until(d => d.FindElement(By.XPath("//h2[text()='Login Activity']")));
                                await Task.Delay(467);
                                try
                                {
                                    js.ExecuteScript("document.evaluate(\"//div[text()='This Was Me']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                    await Task.Delay(897);
                                    js.ExecuteScript("document.evaluate(\"//button[text()='Confirm']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                    await Task.Delay(1297);
                                    js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Home']//ancestor::a[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                }
                                catch
                                {
                                    js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Home']//ancestor::a[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                }
                            }
                        }
                        catch (Exception err)
                        {
                            LogMessage(err.Message);
                        }
                        ret.Status = 1;
                        ret.Response = "Sucesso ao entrar na conta";
                        return ret;
                    case 1:
                        ret.Status = 0;
                        ret.Response = "Challenge";
                        return ret;
                    case 2:
                        ret.Status = 2;
                        ret.Response = "Dados incorretos";
                        return ret;
                    default:
                        try
                        {
                            TakeScreenShot($"logar-{Contas[Index].Username}-{HorarioString()}");
                        }
                        catch { }
                        ret.Status = -1;
                        ret.Response = "Erro ao carregar o perfil";
                        return ret;
                }
            }
            catch (Exception err)
            {
                LogMessage($"Erro Login Insta: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Realizar a humanização inicial
        /// </summary>
        /// <returns></returns>
        private async Task<Retorno> HumanizacaoInicial()
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                if (Driver.Url.IndexOf("instagram.com") > -1)
                {
                    try
                    {
                        Driver.FindElement(By.XPath("//img[@alt='Instagram']/ancestor::a[1]")).Click();
                    }
                    catch { }
                }
                await Task.Delay(2187);
                if (Driver.Url != "https://www.instagram.com/")
                {
                    Driver.Navigate().GoToUrl("https://www.instagram.com/");
                }
                await Task.Delay(1348);
                var random = new Random();
                IWebElement element = Driver.FindElement(By.CssSelector("body"));
                int aux = random.Next(15, 40);
                for (int j = 0; j < aux; j++)
                {
                    element.SendKeys(Keys.ArrowDown);
                }
                await Task.Delay(TimeSpan.FromSeconds(random.Next(2, 5)));
                aux = random.Next(15, 40);
                for (int j = 0; j < aux; j++)
                {
                    element.SendKeys(Keys.ArrowDown);
                }
                await Task.Delay(TimeSpan.FromSeconds(random.Next(2, 7)));
                var elements = Driver.FindElements(By.XPath("//*[name()='svg'][@aria-label='Curtir'][@height='24']/ancestor::button[1]"));
                if (elements == null)
                    elements = Driver.FindElements(By.XPath("//*[name()='svg'][@aria-label='Like'][@height='24']/ancestor::button[1]"));
                if (elements.Count < 1)
                    elements = Driver.FindElements(By.XPath("//*[name()='svg'][@aria-label='Like'][@height='24']/ancestor::button[1]"));
                int l = random.Next(0, elements.Count);
                Actions actions = new Actions(Driver);
                actions.MoveToElement(elements[l]);
                actions.Perform();
                await Task.Delay(1349);
                elements[l].Click();
                await Task.Delay(1354);
                try
                {
                    Driver.FindElement(By.XPath("//img[@alt='Instagram']/ancestor::a[1]")).Click();
                }
                catch { }
                await Task.Delay(1367);
                elements = Driver.FindElements(By.XPath("//li//div//button//div"));
                l = random.Next(0, elements.Count);
                elements[l].Click();
                await Task.Delay(9437);
                element.SendKeys(Keys.Escape);
                try
                {
                    Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Fechar']/ancestor::button[1]")).Click();
                }
                catch { }
                try
                {
                    Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Close']/ancestor::button[1]")).Click();
                }
                catch { }
                ret.Status = 1;
                ret.Response = "Sucesso";
            }
            catch (Exception err)
            {
                LogMessage($"Erro Curtir Insta: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Rolar o feed apenas 1 vez
        /// </summary>
        /// <returns></returns>
        private async Task<Retorno> Feed1()
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                if (Driver.Url.IndexOf("instagram.com") > -1)
                {
                    try
                    {
                        Driver.FindElement(By.XPath("//img[@alt='Instagram']/ancestor::a[1]")).Click();
                    }
                    catch { }
                }
                await Task.Delay(1974);
                if (Driver.Url != "https://www.instagram.com/")
                {
                    Driver.Navigate().GoToUrl("https://www.instagram.com/");
                }
                await Task.Delay(1348);
                var random = new Random();
                IWebElement element = Driver.FindElement(By.CssSelector("body"));
                int aux = random.Next(20, 55);
                for (int j = 0; j < aux; j++)
                {
                    element.SendKeys(Keys.ArrowDown);
                }
                await Task.Delay(TimeSpan.FromSeconds(random.Next(3, 7)));
                ret.Status = 1;
                ret.Response = "Sucesso";
            }
            catch (Exception err)
            {
                LogMessage($"Erro Curtir Insta: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Rolar o feed 2 vezes
        /// </summary>
        /// <returns></returns>
        private async Task<Retorno> Feed2()
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                if (Driver.Url.IndexOf("instagram.com") > -1)
                {
                    try
                    {
                        Driver.FindElement(By.XPath("//img[@alt='Instagram']/ancestor::a[1]")).Click();
                    }
                    catch { }
                }
                await Task.Delay(1974);
                if (Driver.Url != "https://www.instagram.com/")
                {
                    Driver.Navigate().GoToUrl("https://www.instagram.com/");
                }
                await Task.Delay(1348);
                var random = new Random();
                IWebElement element = Driver.FindElement(By.CssSelector("body"));
                int aux = random.Next(20, 55);
                for (int j = 0; j < aux; j++)
                {
                    element.SendKeys(Keys.ArrowDown);
                }
                await Task.Delay(TimeSpan.FromSeconds(random.Next(3, 7)));
                aux = random.Next(20, 55);
                for (int j = 0; j < aux; j++)
                {
                    element.SendKeys(Keys.ArrowDown);
                }
                await Task.Delay(TimeSpan.FromSeconds(random.Next(3, 7)));
                ret.Status = 1;
                ret.Response = "Sucesso";
            }
            catch (Exception err)
            {
                LogMessage($"Erro Curtir Insta: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Rolar o feed 2 vezes e curtir uma pblicação
        /// </summary>
        /// <returns></returns>
        private async Task<Retorno> Feed3()
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                if (Driver.Url.IndexOf("instagram.com") > -1)
                {
                    try
                    {
                        Driver.FindElement(By.XPath("//img[@alt='Instagram']/ancestor::a[1]")).Click();
                    }
                    catch { }
                }
                await Task.Delay(1974);
                if (Driver.Url != "https://www.instagram.com/")
                {
                    Driver.Navigate().GoToUrl("https://www.instagram.com/");
                }
                await Task.Delay(1348);
                var random = new Random();
                IWebElement element = Driver.FindElement(By.CssSelector("body"));
                int aux = random.Next(20, 55);
                for (int j = 0; j < aux; j++)
                {
                    element.SendKeys(Keys.ArrowDown);
                }
                await Task.Delay(TimeSpan.FromSeconds(random.Next(3, 7)));
                aux = random.Next(20, 55);
                for (int j = 0; j < aux; j++)
                {
                    element.SendKeys(Keys.ArrowDown);
                }
                await Task.Delay(TimeSpan.FromSeconds(random.Next(3, 7)));
                var elements = Driver.FindElements(By.XPath("//*[name()='svg'][@aria-label='Curtir'][@height='24']/ancestor::button[1]"));
                if (elements == null)
                    elements = Driver.FindElements(By.XPath("//*[name()='svg'][@aria-label='Like'][@height='24']/ancestor::button[1]"));
                if (elements.Count < 1)
                    elements = Driver.FindElements(By.XPath("//*[name()='svg'][@aria-label='Like'][@height='24']/ancestor::button[1]"));
                int l = random.Next(0, elements.Count);
                Actions actions = new Actions(Driver);
                actions.MoveToElement(elements[l]);
                actions.Perform();
                await Task.Delay(1349);
                elements[l].Click();
                await Task.Delay(1354);
                try
                {
                    Driver.FindElement(By.XPath("//img[@alt='Instagram']/ancestor::a[1]")).Click();
                }
                catch { }
                await Task.Delay(1367);
                ret.Status = 1;
                ret.Response = "Sucesso";
            }
            catch (Exception err)
            {
                LogMessage($"Erro Curtir Insta: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Assistir story no perfil
        /// </summary>
        /// <returns></returns>
        private async Task<Retorno> Perfil1()
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                try
                {
                    var element = Driver.FindElement(By.XPath("//div[@aria-disabled='false']"));
                    if (element != null)
                    {
                        js.ExecuteScript("document.evaluate(\"//div[@aria-disabled='false']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                        var random = new Random();
                        var secondsToWatch = random.Next(6, 16);
                        await Task.Delay(TimeSpan.FromSeconds(secondsToWatch));
                        try
                        {
                            js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Fechar']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                        }
                        catch
                        {
                            try
                            {
                                js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Close']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            }
                            catch
                            {
                                try
                                {
                                    Driver.FindElement(By.XPath("//button[text()='Seguir']"));
                                }
                                catch
                                { }
                            }
                        }
                    }
                }
                catch
                {
                    try
                    {
                        var element = Driver.FindElement(By.XPath("//div[@aria-label='Abrir o Stories']"));
                        if (element != null)
                        {
                            js.ExecuteScript("document.evaluate(\"//div[@aria-label='Abrir o Stories']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            var random = new Random();
                            var secondsToWatch = random.Next(6, 16);
                            await Task.Delay(TimeSpan.FromSeconds(secondsToWatch));
                            try
                            {
                                js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Fechar']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            }
                            catch
                            {
                                try
                                {
                                    js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Close']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                }
                                catch
                                {
                                    try
                                    {
                                        Driver.FindElement(By.XPath("//button[text()='Seguir']"));
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                    }
                    catch { }
                }
                ret.Status = 1;
                ret.Response = "Sucesso";
            }
            catch (Exception err)
            {
                LogMessage($"Erro Perfil1: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Assistir story e curtir uma publicação aleatória entre as 6 priemiras
        /// </summary>
        /// <returns></returns>
        private async Task<Retorno> Perfil2()
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                var random = new Random();
                try
                {
                    var element = Driver.FindElement(By.XPath("//div[@aria-disabled='false']"));
                    if (element != null)
                    {
                        js.ExecuteScript("document.evaluate(\"//div[@aria-disabled='false']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                        var secondsToWatch = random.Next(6, 16);
                        await Task.Delay(TimeSpan.FromSeconds(secondsToWatch));
                        try
                        {
                            js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Fechar']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                        }
                        catch
                        {
                            try
                            {
                                js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Close']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            }
                            catch
                            {
                                try
                                {
                                    Driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Escape);
                                }
                                catch
                                {
                                    try
                                    {
                                        Driver.FindElement(By.XPath("//button[text()='Seguir']"));
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    try
                    {
                        var element = Driver.FindElement(By.XPath("//div[@aria-label='Abrir o Stories']"));
                        if (element != null)
                        {
                            js.ExecuteScript("document.evaluate(\"//div[@aria-label='Abrir o Stories']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            var secondsToWatch = random.Next(6, 16);
                            await Task.Delay(TimeSpan.FromSeconds(secondsToWatch));
                            try
                            {
                                js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Fechar']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            }
                            catch
                            {
                                try
                                {
                                    js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Close']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                }
                                catch
                                {
                                    try
                                    {
                                        Driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Escape);
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            Driver.FindElement(By.XPath("//button[text()='Seguir']"));
                                        }
                                        catch
                                        { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
                try
                {
                    var elements = Driver.FindElements(By.XPath("//div//div//div//a//div//div//img//ancestor::a[1]"));
                    if (elements != null)
                    {
                        if (elements.Count > 0)
                        {
                            var post = random.Next(0, elements.Count);
                            Actions action = new Actions(Driver);
                            action.MoveToElement(elements[post]);
                            action.Perform();
                            await Task.Delay(425);
                            elements[post].Click();
                            await Task.Delay(843);
                            try
                            {
                                Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Curtir'][@height='24']/ancestor::button[1]")).Click();
                            }
                            catch
                            {
                                try
                                {
                                    Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Like'][@height='24']/ancestor::button[1]")).Click();
                                }
                                catch { }
                            }
                            await Task.Delay(349);
                            try
                            {
                                js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Fechar']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            }
                            catch
                            {
                                try
                                {
                                    js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Close']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                }
                                catch
                                {
                                    try
                                    {
                                        Driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Escape);
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            Driver.FindElement(By.XPath("//button[text()='Seguir']"));
                                        }
                                        catch
                                        { }
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
                ret.Status = 1;
                ret.Response = "Sucesso";
            }
            catch (Exception err)
            {
                LogMessage($"Erro Perfil2: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Assistir story curtir e comentar uma publicação aleatória entra as 6 primeiras
        /// </summary>
        /// <returns></returns>
        private async Task<Retorno> Perfil3()
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                var random = new Random();
                try
                {
                    var element = Driver.FindElement(By.XPath("//div[@aria-disabled='false']"));
                    if (element != null)
                    {
                        js.ExecuteScript("document.evaluate(\"//div[@aria-disabled='false']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                        var secondsToWatch = random.Next(6, 16);
                        await Task.Delay(TimeSpan.FromSeconds(secondsToWatch));
                        try
                        {
                            js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Fechar']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                        }
                        catch
                        {
                            try
                            {
                                js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Close']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            }
                            catch
                            {
                                try
                                {
                                    Driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Escape);
                                }
                                catch
                                {
                                    try
                                    {
                                        Driver.FindElement(By.XPath("//button[text()='Seguir']"));
                                    }
                                    catch
                                    { }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    try
                    {
                        var element = Driver.FindElement(By.XPath("//div[@aria-label='Abrir o Stories']"));
                        if (element != null)
                        {
                            js.ExecuteScript("document.evaluate(\"//div[@aria-label='Abrir o Stories']\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            var secondsToWatch = random.Next(6, 16);
                            await Task.Delay(TimeSpan.FromSeconds(secondsToWatch));
                            try
                            {
                                js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Fechar']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            }
                            catch
                            {
                                try
                                {
                                    js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Close']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                }
                                catch
                                {
                                    try
                                    {
                                        Driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Escape);
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            Driver.FindElement(By.XPath("//button[text()='Seguir']"));
                                        }
                                        catch
                                        { }
                                    }
                                }
                            }
                        }
                    }
                    catch { }
                }
                try
                {
                    var elements = Driver.FindElements(By.XPath("//div//div//div//a//div//div//img//ancestor::a[1]"));
                    if (elements != null)
                    {
                        if (elements.Count > 0)
                        {
                            var post = random.Next(0, elements.Count);
                            Actions action = new Actions(Driver);
                            action.MoveToElement(elements[post]);
                            action.Perform();
                            await Task.Delay(425);
                            elements[post].Click();
                            await Task.Delay(843);
                            try
                            {
                                Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Curtir'][@height='24']/ancestor::button[1]")).Click();
                            }
                            catch
                            {
                                try
                                {
                                    Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Like'][@height='24']/ancestor::button[1]")).Click();
                                }
                                catch { }
                            }
                            await Task.Delay(349);
                            try
                            {
                                var element = Driver.FindElement(By.XPath("//*[name()='section']//div//form//*[name()='textarea']"));
                                if (element != null)
                                {
                                    element.SendKeys("💥🔥");
                                    await Task.Delay(349);
                                    Driver.FindElement(By.XPath("//*[name()='section']//div//form//button[@type='submit']")).Click();
                                    await Task.Delay(1349);
                                }
                            }
                            catch { }
                            try
                            {
                                js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Fechar']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                            }
                            catch
                            {
                                try
                                {
                                    js.ExecuteScript("document.evaluate(\"//*[name()='svg'][@aria-label='Close']/ancestor::button[1]\", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue.click()");
                                }
                                catch
                                {
                                    try
                                    {
                                        Driver.FindElement(By.CssSelector("body")).SendKeys(Keys.Escape);
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            Driver.FindElement(By.XPath("//button[text()='Seguir']"));
                                        }
                                        catch
                                        { }
                                    }
                                }
                            }
                        }
                    }
                }
                catch { }
                ret.Status = 1;
                ret.Response = "Sucesso";
            }
            catch (Exception err)
            {
                LogMessage($"Erro Perfil2: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Seguir um perfil do instagram
        /// </summary>
        /// <param name="Alvo">Username do perfil</param>
        /// <returns>
        /// -1 - Pagina não carrega
        /// 0 - Challenge
        /// 1 - Sucesso
        /// 2 - Confirmar Login
        /// 3 - Ja segue o perfil
        /// 4 - Block Temporario
        /// 5 - ERRO AO SEGUIR
        /// </returns>
        private async Task<Retorno> Seguir(string Alvo, bool first = true)
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                /*
                if (!Global.Barra && first)
                {
                    if (Driver.Url != "https://www.instagram.com/")
                    {
                        Driver.Navigate().GoToUrl("https://www.instagram.com/");
                    }
                    await Task.Delay(1547);
                    wait.Until(d => d.FindElement(By.CssSelector("input[type='text']")));
                    await Task.Delay(698);
                    Driver.FindElement(By.CssSelector("input[type='text']")).SendKeys(Alvo);
                    await Task.Delay(854);
                    var w = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
                    try
                    {
                        w.Until(d => d.FindElement(By.XPath($"//div[text()='{Alvo}']")));
                        Driver.FindElement(By.XPath($"//div[text()='{Alvo}']")).Click();
                        await Task.Delay(1345);
                    }
                    catch
                    {
                        Driver.Navigate().GoToUrl($"https://www.instagram.com/{Alvo}/");
                        await Task.Delay(1248);
                    }
                }
                else
                {
                    Driver.Navigate().GoToUrl($"https://www.instagram.com/{Alvo}/");
                    await Task.Delay(1248);
                }
                */
                var waits = new List<Models.Waits.Wait>
                {
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = $"//h2[text()='{Alvo}']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//h2[text()=='Esta página não está disponível.']" },
                    new Models.Waits.Wait {TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//h2[text()='Sorry, this page isn't available.']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//h3[text()'Adicionar telefone para voltar ao Instagram']" },
                    new Models.Waits.Wait {TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//h3[text()='Add Phone Number to Get Back Into Instagram']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//h2[text()='Confirme que é você fazendo login']" },
                    new Models.Waits.Wait {TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//h2[text()='Confirm that it's you by signing in']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//h2[text()='Erro']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button[text()='Seguir']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button[text()='Seguir de volta']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button[text()='Follow']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button[text()='Follow Back']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button//div[text()='Enviar mensagem']" },
                    new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button//div[text()='Message']" }
                };
                var r = await Models.Waits.Waits.Wait(waits, 10, Driver);
                if (r == 0 || r == 8 || r == 9 || r == 10 || r == 11 || r == 12 || r == 13)
                {
                    waits.Clear();
                    waits.Add(new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button[text()='Seguir']" });
                    waits.Add(new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button[text()='Seguir de volta']" });
                    waits.Add(new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button[text()='Follow']" });
                    waits.Add(new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button[text()='Follow Back']" });
                    waits.Add(new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button//div[text()='Enviar mensagem']" });
                    waits.Add(new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//button//div[text()='Message']" });
                    waits.Add(new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//h2[text()=='Esta página não está disponível.']" });
                    waits.Add(new Models.Waits.Wait { TYPE = Models.Waits.WaitTypes.TYPE_XPATH, VALUE = "//h2[text()='Sorry, this page isn't available.']" });
                    r = await Models.Waits.Waits.Wait(waits, 10, Driver);
                    if (r < 4)
                    {
                        if (first)
                        {
                            var rand = new Random();
                            int p = rand.Next(Global.Delay1, Global.Delay2);
                            p /= 2;
                            await Task.Delay(TimeSpan.FromSeconds(p));
                            if (App.Humanizacao)
                            {
                                if (Porcent(35))
                                {
                                    await ConsoleMessage("Humanização Tipo 1", 7);
                                    _ = await Perfil1();
                                }
                                else
                                {
                                    if (Porcent(15))
                                    {
                                        await ConsoleMessage("Humanização Tipo 2", 7);
                                        _ = await Perfil2();
                                    }
                                    else
                                    {
                                        if (Porcent(5))
                                        {
                                            await ConsoleMessage("Humanização Tipo 3", 7);
                                            _ = await Perfil3();
                                        }
                                    }
                                }
                            }
                        }
                        await Task.Delay(1245);
                        var actions = new Actions(Driver);
                        try
                        {
                            switch (r)
                            {
                                case 0:
                                    var element = Driver.FindElement(By.XPath("//button[text()='Seguir']"));
                                    actions.MoveToElement(element);
                                    actions.Perform();
                                    await Task.Delay(267);
                                    element.Click();
                                    break;
                                case 1:
                                    var element1 = Driver.FindElement(By.XPath("//button[text()='Seguir de volta']"));
                                    actions.MoveToElement(element1);
                                    actions.Perform();
                                    await Task.Delay(267);
                                    element1.Click();
                                    break;
                                case 2:
                                    var element2 = Driver.FindElement(By.XPath("//button[text()='Follow']"));
                                    actions.MoveToElement(element2);
                                    actions.Perform();
                                    await Task.Delay(267);
                                    element2.Click();
                                    break;
                                case 3:
                                    var element3 = Driver.FindElement(By.XPath("//button[text()='Follow Back']"));
                                    actions.MoveToElement(element3);
                                    actions.Perform();
                                    await Task.Delay(267);
                                    element3.Click();
                                    break;
                                default:
                                    break;
                            }
                        }
                        catch (Exception err)
                        {
                            LogMessage($"Erro ao clicar no botão de seguir: {err.Message}");
                        }
                        await Task.Delay(1245);
                        waits.Clear();
                        waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//button//div[text()='Enviar mensagem']" });
                        waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//button//div[text()='Message']" });
                        waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//h3[text()='Tente novamente mais tarde']" });
                        waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//h3[text()='Try again later']" });
                        waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//h3[text()='Você não pode seguir contas no momento']" });
                        waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//h3[text()='You cannot follow accounts at the moment']" });
                        r = await Models.Waits.Waits.Wait(waits, 10, Driver);
                        switch (r)
                        {
                            case 0:
                            case 1:
                                ret.Status = 1;
                                ret.Response = "Sucesso ao seguir perfil";
                                break;
                            case 2:
                            case 3:
                                ret.Status = 4;
                                ret.Response = "Bloqueio temporário";
                                break;
                            case 4:
                            case 5:
                                ret.Status = 4;
                                ret.Response = "Bloqueio temporário";
                                break;
                            default:
                                if (first)
                                {
                                    return await Seguir(Alvo, false);
                                }
                                else
                                {
                                    try
                                    {
                                        TakeScreenShot($"logar-{Contas[Index].Username}-{HorarioString()}");
                                    }
                                    catch { }
                                    ret.Status = -1;
                                    ret.Response = "Erro ao carregar o perfil";
                                    return ret;
                                }
                        }
                    }
                    else
                    {
                        switch (r)
                        {
                            case 4:
                            case 5:
                                ret.Status = 3;
                                ret.Response = "Ja seguia o perfil";
                                break;
                            case 6:
                            case 7:
                                ret.Status = -1;
                                ret.Response = "Não foi possivel carregar o perfil";
                                break;
                            default:
                                if (first)
                                {
                                    return await Seguir(Alvo, false);
                                }
                                else
                                {
                                    try
                                    {
                                        TakeScreenShot($"logar-{Contas[Index].Username}-{HorarioString()}");
                                    }
                                    catch { }
                                    ret.Status = -1;
                                    ret.Response = "Erro ao carregar o perfil";
                                    return ret;
                                }
                        }
                    }
                }
                else
                {
                    switch (r)
                    {
                        case 1:
                        case 2:
                            ret.Status = -1;
                            ret.Response = "Não foi possivel carregar o perfil";
                            break;
                        case 3:
                        case 4:
                            ret.Status = 0;
                            ret.Response = "Bloqueio de SMS";
                            break;
                        case 5:
                        case 6:
                            ret.Status = 2;
                            ret.Response = "Confirmar login";
                            break;
                        case 7:
                            ret.Status = -1;
                            ret.Response = "Náo foi possivel carregar o perfil";
                            break;
                        default:
                            if (first)
                            {
                                return await Seguir(Alvo, false);
                            }
                            else
                            {
                                try
                                {
                                    TakeScreenShot($"logar-{Contas[Index].Username}-{HorarioString()}");
                                }
                                catch { }
                                ret.Status = -1;
                                ret.Response = "Erro ao carregar o perfil";
                                return ret;
                            }
                    }
                }

            }
            catch (Exception err)
            {
                LogMessage($"Erro Seguir Insta: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Curtir uma publicação do instagram
        /// </summary>
        /// <param name="Url">Url da publicação</param>
        /// <returns>
        /// 0 - Challenge
        /// 1 - Sucesso
        /// 2 - Confirmar Login
        /// 3 - Ja segue o perfil
        /// 4 - Block Temporario
        /// 5 - ERRO AO CURTIR
        /// </returns>
        private async Task<Retorno> Curtir(string Url)
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                //Driver.Navigate().GoToUrl($"{Url}");
                await Task.Delay(1248);
                var waits = new List<Models.Waits.Wait>
                {
                    new Models.Waits.Wait { TYPE = 3, VALUE = "//*[name()='svg'][@aria-label='Curtir'][@height='24']"},
                    new Models.Waits.Wait { TYPE = 3, VALUE = "//*[name()='svg'][@aria-label='Like'][@height='24']"},
                    new Models.Waits.Wait { TYPE = 3, VALUE = "//*[name()='svg'][@aria-label='Descurtir'][@height='24']"},
                    new Models.Waits.Wait { TYPE = 3, VALUE = "//*[name()='svg'][@aria-label='Unlike'][@height='24']"},
                    new Models.Waits.Wait { TYPE = 3, VALUE = "//h2[text()='Esta página não está disponível.']"},
                    new Models.Waits.Wait { TYPE = 3, VALUE = "//h2[text()='This page is not available.']"},
                    new Models.Waits.Wait { TYPE = 3, VALUE = "//h3[text()'Adicionar telefone para voltar ao Instagram']" },
                    new Models.Waits.Wait {TYPE = 3, VALUE = "//h3[text()='Add Phone Number to Get Back Into Instagram']" },
                    new Models.Waits.Wait { TYPE = 3, VALUE = "//h2[text()='Confirme que é você fazendo login']" },
                    new Models.Waits.Wait {TYPE = 3, VALUE = "//h2[text()='Confirm that it's you by signing in']" },
                    new Models.Waits.Wait { TYPE = 3, VALUE = "//h2[text()='Erro']" }
                };
                var r = await Models.Waits.Waits.Wait(waits, 15, Driver);
                await Task.Delay(458);
                if (r < 2)
                {
                    var rand = new Random();
                    int p = rand.Next(Global.Delay1, Global.Delay2);
                    p /= 2;
                    await Task.Delay(TimeSpan.FromSeconds(p));
                    try
                    {
                        var actions = new Actions(Driver);
                        if (r == 0)
                        {
                            var element = Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Curtir'][@height='24']/ancestor::button[1]"));
                            try
                            {
                                actions.MoveToElement(element);
                                actions.Perform();
                                await Task.Delay(341);
                            }
                            catch { }
                            element.Click();
                        }
                        else
                        {
                            var element = Driver.FindElement(By.XPath("//*[name()='svg'][@aria-label='Like'][@height='24']/ancestor::button[1]"));
                            try
                            {
                                actions.MoveToElement(element);
                                actions.Perform();
                                await Task.Delay(341);
                            }
                            catch { }
                            element.Click();
                        }
                    }
                    catch (Exception err)
                    {
                        LogMessage($"Erro ao curtir publicação: {err.Message}");
                    }
                    await Task.Delay(1342);
                    waits.Clear();
                    waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//*[name()='svg'][@aria-label='Descurtir'][@height='24']" });
                    waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//*[name()='svg'][@aria-label='Unlike'][@height='24']" });
                    waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//h3[text()='Tente novamente mais tarde']" });
                    waits.Add(new Models.Waits.Wait { TYPE = 3, VALUE = "//h3[text()='Try Again Later']" });
                    r = await Models.Waits.Waits.Wait(waits, 5, Driver);
                    await Task.Delay(1245);
                    switch (r)
                    {
                        case 0:
                        case 1:
                            ret.Status = 1;
                            ret.Response = "Sucesso ao curtir";
                            break;
                        case 2:
                        case 3:
                            ret.Status = 4;
                            ret.Response = "Bloqueio temporário";
                            break;
                        default:
                            try
                            {
                                TakeScreenShot($"logar-{Contas[Index].Username}-{HorarioString()}");
                            }
                            catch { }
                            ret.Status = -1;
                            ret.Response = "Erro ao carregar o perfil";
                            return ret;
                    }
                }
                else
                {
                    switch (r)
                    {
                        case 2:
                        case 3:
                            ret.Status = 3;
                            ret.Response = "Ja curtiu a publicação";
                            break;
                        case 4:
                        case 5:
                            ret.Status = -1;
                            ret.Response = "Não foi possivel carregar a pagina";
                            break;
                        case 6:
                        case 7:
                            ret.Status = 0;
                            ret.Response = "Bloqueio de SMS";
                            break;
                        case 8:
                        case 9:
                            ret.Status = 2;
                            ret.Response = "Confirmar login";
                            break;
                        default:
                            try
                            {
                                TakeScreenShot($"logar-{Contas[Index].Username}-{HorarioString()}");
                            }
                            catch { }
                            ret.Status = -1;
                            ret.Response = "Erro ao carregar o perfil";
                            return ret;
                    }
                }
            }
            catch (Exception err)
            {
                LogMessage($"Erro Curtir Insta: {err.Message}");
            }
            return ret;
        }
        /// <summary>
        /// Pegar a imagem de Profile da conta atual
        /// </summary>
        /// <returns></returns>
        private async Task<Retorno> ProfileImage()
        {
            Retorno ret = new Retorno
            {
                Status = 0,
                Response = ""
            };
            try
            {
                if (Driver.Url != "https://www.instagram.com/")
                    Driver.Navigate().GoToUrl($"https://www.instagram.com/");
                await Task.Delay(1454);
                wait.Until(d => d.FindElement(By.CssSelector("img[data-testid=user-avatar]")));
                await Task.Delay(345);
                ret.Response = Driver.FindElement(By.XPath($"//nav//div//div//div//div//div//div//span//img")).GetAttribute("src");
                ret.Status = 1;
            }
            catch (Exception err)
            {
                LogMessage($"Erro ProfileImage Insta: {err.Message}");
            }
            return ret;
        }

        #endregion

        #region Navegador

        private async Task<bool> AbrirNavegador()
        {
            try
            {
                await Task.Delay(500);
                var dir = Directory.GetCurrentDirectory();
                var directory = $@"{dir}\Bot\navegador";
                ChromeOptions Options = null;
                Options = new ChromeOptions();
                var chromeDriverService = ChromeDriverService.CreateDefaultService(directory);
                Options.BinaryLocation = $@"{dir}\Bot\navegador\chrome.exe";
                Options.AddArguments("--log-level=3");
                chromeDriverService.HideCommandPromptWindow = true;
                if (Global.Anonimo)
                {
                    Options.AddArguments("--incognito");
                }
                Options.AddArguments("--headless");
                Options.AddArguments("--mute-audio");
                Options.AddArgument("--disable-gpu");
                Options.AddArgument("--disable-gpu-vsync");
                Options.AddArgument("--window-size=800,600");
                Options.AddArgument("--blink-settings=imagesEnabled=false");
                switch (App.Nav)
                {
                    case "google":
                        Options.AddArguments($"--user-agent={UserAgentController.GetUserChrome()}");
                        Driver = new ChromeDriver(chromeDriverService, Options);
                        wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(80));
                        js = (IJavaScriptExecutor)Driver;
                        return true;
                    case "edge":
                        Options.AddArguments($"--user-agent={UserAgentController.GetUserEdge()}");
                        Driver = new ChromeDriver(chromeDriverService, Options);
                        wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(80));
                        js = (IJavaScriptExecutor)Driver;
                        return true;
                    case "brave":
                        Options.AddArguments($"--user-agent={UserAgentController.GetUserBrave()}");
                        Driver = new ChromeDriver(chromeDriverService, Options);
                        wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(80));
                        js = (IJavaScriptExecutor)Driver;
                        return true;
                    default:
                        return false;
                }
            }
            catch (Exception err)
            {
                LogMessage(" O Seguinte erro aconteceu ao abrir o navegador: " + err.Message.ToString());
                await ConsoleMessage("Não foi possivel abrir o navegador", 5);
                return false;
            }
        }

        #endregion

        #region Menu / Design

        private void MenuStrip_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuStrip.Visibility = Visibility.Hidden;
        }

        private void MenuInicio_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TelaDados.Visibility = Visibility.Visible;
            TelaConsole.Visibility = Visibility.Hidden;
            TelaLista.Visibility = Visibility.Hidden;
            MenuStrip.Visibility = Visibility.Hidden;
        }

        private void MenuConsole_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TelaDados.Visibility = Visibility.Hidden;
            TelaConsole.Visibility = Visibility.Visible;
            TelaLista.Visibility = Visibility.Hidden;
            MenuStrip.Visibility = Visibility.Hidden;
        }

        private void MenuContas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            List<InstaList> lsit = new List<InstaList>();
            foreach (var i in Contas)
            {
                var aux = new InstaList();
                aux.Username = i.Username;
                aux.Color = i.Incorrect ? IncorrectCollor : i.Challeng ? ChallengeCollor : i.Block ? BlockCollor : NormalCollor;
                if (InstagramGNI.Exists(c => c.Insta.Username == i.Username))
                {
                    var j = InstagramGNI.FindIndex(c => c.Insta.Username == i.Username);
                    aux.Seguir = InstagramGNI[j].Seguir.ToString();
                    aux.Curtir = InstagramGNI[j].Curtir.ToString();
                    aux.Url = InstagramGNI[j].PictureURL;
                }
                else
                {
                    aux.Seguir = "0";
                    aux.Curtir = "0";
                    aux.Url = "https://imgur.com/b24Rzo7.jpg";
                }
                lsit.Add(aux);
            }
            ListaContasList.ItemsSource = null;
            ListaContasList.ItemsSource = lsit;
            TelaDados.Visibility = Visibility.Hidden;
            TelaConsole.Visibility = Visibility.Hidden;
            TelaLista.Visibility = Visibility.Visible;
            MenuStrip.Visibility = Visibility.Hidden;
        }

        private void bt_menu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuStrip.Visibility = Visibility.Visible;
        }

        private void BotBorderTop_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Driver.Quit();
            }
            catch { }
            System.Windows.Application.Current.Shutdown();
        }

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        #endregion

        #region Funções
        // 1 = Roxo #221154 : 2 = Verde #07BB9F : 3 = Amarelo #F5C300 : 4 = Vermelho #F03C22 : 5 Rosa = #FE8495 : 6 = Azul #0052cc : 7 = Laranja #FD4731
        private async Task ConsoleMessage(string message, int c)
        {
            var color = c == 1 ? "#221154" : c == 2 ? "#07BB9F" : c == 3 ? "#F5C300" : c == 4 ? "#F03C22" : c == 5 ? "#FE8495" : c == 6 ? "#0052cc" : "#FD4731";
            var bc = new BrushConverter();
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(message);
            paragraph.Foreground = (Brush)bc.ConvertFrom(color);
            BotConsole.Document.Blocks.Add(paragraph);
            BotConsole.Focus();
            BotConsole.ScrollToEnd();
            await Task.Delay(300);
        }

        private async Task SetBorder(int c)
        {
            var color = c == 1 ? "#221154" : c == 2 ? "#07BB9F" : c == 3 ? "#F5C300" : c == 4 ? "#F03C22" : c == 5 ? "#FE8495" : "#0052cc";
            var bc = new BrushConverter();
            BorderImage.BorderBrush = (Brush)bc.ConvertFrom(color);
            await Task.Delay(300);
        }

        /// <summary>
        /// Adicionar uma tarefa na conta atual, sendo 0 para seguir e 1 para curtir
        /// </summary>
        /// <param name="type">Tipo de tarefa a adicionar</param>
        /// <param name="i">Index do InstagramGNI</param>
        /// <returns>Nada</returns>
        private async Task AddTarefa(int type, int i)
        {
            Total++;
            Meta++;
            if (type == 0)
            {
                Contas[Index].Seguir++;
                Contas[Index].AdicionarSeguir();
                InstagramGNI[i].Seguir++;
                InstagramGNI[i].Total++;
                tx_tconta.Text = InstagramGNI[i].Total.ToString();
                tx_seguir.Text = InstagramGNI[i].Seguir.ToString();
            }
            else
            {
                Contas[Index].Curtir++;
                Contas[Index].AdicionarCurtir();
                InstagramGNI[i].Curtir++;
                InstagramGNI[i].Total++;
                tx_tconta.Text = InstagramGNI[i].Total.ToString();
                tx_curtir.Text = InstagramGNI[i].Curtir.ToString();
            }
            tx_total.Text = Total.ToString();
            tx_meta.Text = $"{Meta}/{Global.Meta}";
            if (Meta >= Global.Meta)
            {
                await SetBorder(6);
                await ConsoleMessage("Meta de tarefas alcançada.", 6);
                await ConsoleMessage($"Aguardando {Global.Timer_Meta} minutos para continuar", 6);
                await Task.Delay(TimeSpan.FromMinutes(Global.Timer_Meta));
                Meta = 0;
            }
        }

        private async Task AddSaldo(double Valor)
        {
            Saldo += Valor;
            tx_saldo.Text = $"${Saldo.ToString("N3")}";
            await Task.Delay(102);
        }

        private void Show(int i)
        {
            tx_seguir.Text = InstagramGNI[i].Seguir.ToString();
            tx_curtir.Text = InstagramGNI[i].Curtir.ToString();
            tx_tconta.Text = InstagramGNI[i].Total.ToString();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            DateTime agora = DateTime.Now;
            TimeSpan span = agora.Subtract(inicio);
            string time = $"{span.ToString(@"hh\:mm\:ss")}";
            tx_timer.Text = time;
        }

        private string DateString()
        {
            var data = DateTime.Today;
            var dia = data.Day.ToString();
            var mes = data.Month.ToString();
            var ano = data.Year.ToString();
            return $"{dia}-{mes}-{ano}";
        }

        private string HorarioString()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        private void LogMessage(string message)
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();
                if (Directory.Exists($@"{dir}\logs"))
                {
                    var data = DateString();
                    if (File.Exists($@"{dir}\logs\{data}.txt"))
                    {
                        string[] linhas = File.ReadAllLines($@"{dir}\logs\{data}.txt");
                        var list = linhas.ToList();
                        list.Add($"KZOMNAV {HorarioString()} {message}");
                        File.WriteAllLines($@"{dir}\logs\{data}.txt", list);
                        return;
                    }
                    else
                    {
                        string[] linhas = { $"KZOMNAV {HorarioString()} {message}" };
                        File.WriteAllLines($@"{dir}\logs\{data}.txt", linhas);
                        return;
                    }
                }
                else
                {
                    Directory.CreateDirectory($@"{dir}\logs");
                    var data = DateString();
                    if (File.Exists($@"{dir}\logs\{data}.txt"))
                    {
                        string[] linhas = File.ReadAllLines($@"{dir}\logs\{data}.txt");
                        var list = linhas.ToList();
                        list.Add($"KZOMNAV {HorarioString()} {message}");
                        File.WriteAllLines($@"{dir}\logs\{data}.txt", list);
                        return;
                    }
                    else
                    {
                        string[] linhas = { $"KZOMNAV {HorarioString()} {message}" };
                        File.WriteAllLines($@"{dir}\logs\{data}.txt", linhas);
                        return;
                    }
                }
            }
            catch { }
        }

        private bool Porcent(int p)
        {
            var r = new Random();
            int rd = r.Next(1, 300);
            if (p >= rd)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void TakeScreenShot(string SSName)
        {
            try
            {
                var dir = Directory.GetCurrentDirectory();
                string path = $@"{dir}\logs\";
                Screenshot ss = ((ITakesScreenshot)Driver).GetScreenshot();
                ss.SaveAsFile((path + SSName));
            }
            catch
            { }
        }

        #endregion

        #region Inicio

        private async void InitializeDados()
        {
            ProfilePic.ImageSource = new BitmapImage(new Uri(@"https://imgur.com/b24Rzo7.jpg"));
            await Task.Delay(1000);
            if (String.IsNullOrEmpty(App.GrupoName))
            {
                //Close
                LogMessage("Não foi possivel carregar o nome do grupo");
                System.Windows.Application.Current.Shutdown();
                return;
            }
            await Task.Delay(1349);
            Models.Grupos.Grupo ret = null;
            if (App.IsGrupo)
            {
                ret = Models.Grupos.ExtendGrupos.GetGroupByname(App.GrupoName); ;
            } else
            {
                ret = new Models.Grupos.Grupo
                {
                    Contas = App.Contas,
                    Global = App.GrupoName,
                    Nome = App.GrupoName
                };
            }
            if (ret == null)
            {
                //Close
                LogMessage($"Não foi possivel localizar o grupo com nome '{App.GrupoName}'");
                System.Windows.Application.Current.Shutdown();
                return;
            }
            Grupo = ret;
            if (Grupo.Contas.Count <= 0)
            {
                LogMessage($"O grupo '{App.GrupoName}' não possui contas para rodar o bot");
                System.Windows.Application.Current.Shutdown();
                return;
            }
            var resGlobal = Models.Globals.ExtendsGlobal.GetGlobalByname(Grupo.Global);
            if (resGlobal == null)
            {
                LogMessage($"Não foi possivel localizar o global com nome '{Grupo.Global}'");
                System.Windows.Application.Current.Shutdown();
                return;
            }
            Global = resGlobal;
            Contas = new List<Models.Instagrams.Instagram>();
            foreach (string username in Grupo.Contas)
            {
                var res = Models.Instagrams.ExtendInstagram.GetInstaByUsername(username);
                if (res != null)
                {
                    Contas.Add(res);
                }
            }
            if (Contas.Count > 0)
            {
                _ = IniciarSistema();
            }
            else
            {
                LogMessage($"Não foi possivel carregar suas contas do instagram.");
                System.Windows.Application.Current.Shutdown();
                return;
            }
            //ProfilePic.ImageSource = new BitmapImage(new Uri(@"https://instagram.fmvs3-1.fna.fbcdn.net/v/t51.2885-19/s320x320/107820682_927217014441014_5024921085230906180_n.jpg?_nc_ht=instagram.fmvs3-1.fna.fbcdn.net&_nc_ohc=hrYz_6j3Hu4AX_uB6fk&edm=ABfd0MgBAAAA&ccb=7-4&oh=48b50d4452d3508ced88f26e9e90211a&oe=613F64A2&_nc_sid=7bff83"));
        }

        private async Task IniciarSistema()
        {
            await ConsoleMessage("Todos os dados foram carregados do servidor, iniciando o sistema.", 1);
            await Task.Delay(1000);
            await ConsoleMessage("Abrindo o navegador", 1);
            var nav = await AbrirNavegador();
            if (nav)
            {
                await ConsoleMessage("Realizando login na plataforma.", 1);
                var login = await LoginKzom();
                if (login)
                {
                    await ConsoleMessage("Login realizado", 2);
                    _ = RodarCiclo();
                    return;
                }
                else
                {
                    await ConsoleMessage("Não foi possivel logar na plataforma", 5);
                    return;
                }
            }
            else
            {
                return;
            }
        }

        #endregion

        #region Rodar Ciclo / Conta

        private async Task RodarCiclo()
        {
            BotGrupoName.Text = $"Grupo: {Grupo.Nome}";
            Bloqueios = new List<Block>();
            Incorrects = new List<string>();
            Challenges = new List<string>();
            InstagramGNI = new List<InstaGNI>();
            await ConsoleMessage("Login efetuado na plataforma", 1);
            await Task.Delay(500);
            await ConsoleMessage("Iniciando o bot", 1);
            try
            {
                while (true)
                {
                    for (int i = 0; i < Contas.Count; i++)
                    {
                        await SetBorder(1);
                        if (Contas[i].Block)
                        {
                            await Bloqueios.LimparLista();
                            if (Bloqueios.Exists(c => c.Username == Contas[i].Username))
                            {
                                await ConsoleMessage($"Conta '{Contas[i].Username}' está bloqueada temporariamente.", 3);
                            }
                            else
                            {
                                //Rodar conta
                                if (!Challenges.Exists(c => c == Contas[i].Username) && !Incorrects.Exists(c => c == Contas[i].Username))
                                {
                                    //Rodar conta
                                    await ConsoleMessage($"Conta: '{Contas[i].Username}'", 1);
                                    Index = i;
                                    await RodarConta();
                                }
                            }
                        }
                        else
                        {
                            if (!Challenges.Exists(c => c == Contas[i].Username) && !Incorrects.Exists(c => c == Contas[i].Username))
                            {
                                //Rodar conta
                                await ConsoleMessage($"Conta: '{Contas[i].Username}'", 1);
                                Index = i;
                                await RodarConta();
                            }
                        }
                        if (Contas.Count < 2)
                        {
                            if (Challenges.Count == Contas.Count)
                            {
                                await ConsoleMessage("Não possui contas para continuar", 5);
                                await SetBorder(5);
                                await Task.Delay(TimeSpan.FromHours(10));
                                System.Windows.Application.Current.Shutdown();
                                return;
                            }
                            else
                            {
                                if ((Bloqueios.Count + Challenges.Count) == Contas.Count)
                                {
                                    await ConsoleMessage("Não possui contas para continuar", 5);
                                    await ConsoleMessage("Aguardando 10 minutos", 5);
                                    await SetBorder(5);
                                    await Task.Delay(TimeSpan.FromMinutes(10));
                                }
                            }
                        }
                        else
                        {
                            int t = 0;
                            foreach (var ig in Contas)
                            {
                                if (!ig.Challeng && !ig.Incorrect)
                                {
                                    t++;
                                }
                            }
                            if (t == 0)
                            {
                                await ConsoleMessage("Não possui contas para continuar", 5);
                                await SetBorder(5);
                                await Task.Delay(TimeSpan.FromHours(10));
                                System.Windows.Application.Current.Shutdown();
                                return;
                            }
                            else
                            {
                                if ((Bloqueios.Count + Challenges.Count) == Contas.Count)
                                {
                                    await ConsoleMessage("Não possui contas para continuar", 5);
                                    await ConsoleMessage("Aguardando 10 minutos", 5);
                                    await SetBorder(5);
                                    await Task.Delay(TimeSpan.FromMinutes(10));
                                }
                            }
                        }
                        await SetBorder(6);
                        await ConsoleMessage("Aguardando tempo entre contas", 6);
                        await Task.Delay(TimeSpan.FromSeconds(Global.Timer_contas));
                    }
                }
            }
            catch (Exception err)
            {
                LogMessage($"Erro ao rodar 'Ciclo' : {err.Message}");
                System.Windows.Application.Current.Shutdown();
                return;
            }
        }

        private async Task RodarConta()
        {
            tx_username.Text = "@" + Contas[Index].Username;
            int i = -1;
            if (InstagramGNI.Exists(ig => ig.Insta.Username == Contas[Index].Username))
            {
                i = InstagramGNI.FindIndex(inst => inst.Insta.Username == Contas[Index].Username);
            }
            if (i < 0)
            {
                var res = await CheckAccountKzom(Contas[Index].Username);
                if (res.Status == 1)
                {
                    InstaGNI ig = new InstaGNI
                    {
                        Insta = Contas[Index],
                        isLogged = false,
                        PictureURL = "https://imgur.com/b24Rzo7.jpg",
                        Seguir = 0,
                        Curtir = 0,
                        Total = 0
                    };
                    InstagramGNI.Add(ig);
                    i = InstagramGNI.Count - 1;
                }
                else
                {
                    await ConsoleMessage($"Náo foi possivel localizar a conta '{Contas[Index].Username}' no site do GNI", 5);
                    if (Contas.Count > 1)
                        InstagramGNI[i].isLogged = false;
                    return;
                }
            }
            try
            {
                if (!InstagramGNI[i].isLogged)
                {
                    await ConsoleMessage($"Entrando na conta '{Contas[Index].Username}'", 1);
                    js.ExecuteScript("window.open('', '_blank'); ");
                    await Task.Delay(1497);
                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                    await Task.Delay(478);
                    var login = await Login(InstagramGNI[i].Insta);
                    if (login.Status == 1)
                    {
                        InstagramGNI[i].isLogged = true;
                        Contas[Index].RemoverChallenge();
                        Contas[Index].RemoverIncorrect();
                        Contas[Index].Challeng = false;
                        Contas[Index].Incorrect = false;
                        await ConsoleMessage("Login realizado com sucesso", 2);
                        await Task.Delay(500);
                        if (Driver.WindowHandles.Count > 1)
                        {
                            Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                            await Task.Delay(478);
                            Driver.Close();
                            Driver.SwitchTo().Window(Driver.WindowHandles.First());
                            await Task.Delay(478);
                        }
                    }
                    else
                    {
                        switch (login.Status)
                        {
                            case 0:
                                await ConsoleMessage("Conta com block de SMS", 4);
                                await Task.Delay(1000);
                                Contas[Index].Challeng = true;
                                Contas[Index].AdicionarChallenge();
                                InstagramGNI[i].isLogged = false;
                                Challenges.Add(Contas[Index].Username);
                                return;
                            case 2:
                                await ConsoleMessage("Dado de login errado", 5);
                                await Task.Delay(1000);
                                Contas[Index].Incorrect = true;
                                Contas[Index].AdicionarIncorrect();
                                InstagramGNI[i].isLogged = false;
                                Incorrects.Add(Contas[Index].Username);
                                return;
                            default:
                                await ConsoleMessage("Não foi possivel realizar login na conta", 5);
                                await ConsoleMessage("Mensagem: " + login.Response, 5);
                                Contas[Index].AdicionarIncorrect();
                                Contas[Index].Incorrect = true;
                                InstagramGNI[i].isLogged = false;
                                Incorrects.Add(Contas[Index].Username);
                                await Task.Delay(1000);
                                return;
                        }
                    }
                }
                await Task.Delay(471);
                js.ExecuteScript("window.open('', '_blank'); ");
                await Task.Delay(1497);
                Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                await Task.Delay(147);
                var image = await ProfileImage();
                if (image.Status == 1)
                    InstagramGNI[i].PictureURL = image.Response;
                ProfilePic.ImageSource = new BitmapImage(new Uri(InstagramGNI[i].PictureURL));
                if (Driver.WindowHandles.Count > 1)
                {
                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                    await Task.Delay(478);
                    Driver.Close();
                    Driver.SwitchTo().Window(Driver.WindowHandles.First());
                    await Task.Delay(478);
                }
                Show(i);
                int k = 0;
                int frist = 0;
                bool sair = false;
                bool primeira = true;
                var r = new Random();
                while (k < Global.Quantidade && sair == false)
                {
                    await SetBorder(1);
                    if (App.Humanizacao)
                    {
                        if (k == 0 && primeira)
                        {
                            await Task.Delay(471);
                            js.ExecuteScript("window.open('', '_blank'); ");
                            await Task.Delay(1497);
                            Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                            await Task.Delay(147);
                            await ConsoleMessage("Humanização inicial", 7);
                            _ = await HumanizacaoInicial();
                            primeira = false;
                        }
                        else
                        {
                            if (Porcent(35))
                            {
                                await Task.Delay(471);
                                js.ExecuteScript("window.open('', '_blank'); ");
                                await Task.Delay(1497);
                                Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                await Task.Delay(147);
                                await ConsoleMessage("Humanização 1", 7);
                                _ = await Feed1();
                            }
                            else
                            {
                                if (Porcent(13))
                                {
                                    await Task.Delay(471);
                                    js.ExecuteScript("window.open('', '_blank'); ");
                                    await Task.Delay(1497);
                                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                    await Task.Delay(147);
                                    await ConsoleMessage("Humanização 2", 7);
                                    _ = await Feed2();
                                }
                                else
                                {
                                    if (Porcent(5))
                                    {
                                        await Task.Delay(471);
                                        js.ExecuteScript("window.open('', '_blank'); ");
                                        await Task.Delay(1497);
                                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                        await Task.Delay(147);
                                        await ConsoleMessage("Humanização 3", 7);
                                        _ = await Feed3();
                                    }
                                }
                            }
                        }
                    }
                    if (Driver.WindowHandles.Count > 1)
                    {
                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                        await Task.Delay(478);
                        Driver.Close();
                        Driver.SwitchTo().Window(Driver.WindowHandles.First());
                        await Task.Delay(478);
                    }
                    await ConsoleMessage($"Buscando tarefa {k + 1}/{Global.Quantidade}", 1);
                    var task = await GetTaskKzom(Contas[Index].Username.ToLower());
                    if (task.Status == 1 || task.Status == 2)
                    {
                        await ConsoleMessage($"Tarefa encontrada | Tipo: {task.Response} | Alvo: {Driver.Url}", 1);
                        await ConsoleMessage($"Realizando a tarefa", 1);
                        var res = await RealizarTarefa(task, i);
                        if (res.Status != 1)
                        {
                            switch (res.Status)
                            {
                                case -1:
                                    await ConsoleMessage(res.Response, 5);
                                    break;
                                case 0:
                                    await ConsoleMessage(res.Response, 4);
                                    sair = true;
                                    return;
                                case 2:
                                    await ConsoleMessage(res.Response, 4);
                                    return;
                                case 3:
                                    await ConsoleMessage(res.Response, 5);
                                    break;
                                case 4:
                                    await ConsoleMessage(res.Response, 3);
                                    sair = true;
                                    return;
                                case 5:
                                    await ConsoleMessage(res.Response, 5);
                                    break;
                                default:
                                    await ConsoleMessage(res.Response, 5);
                                    break;
                            }
                            await ConsoleMessage("Não foi possivel realizar a tarefa", 4);
                            var ret = await PularAcao(Contas[Index].Username);
                            if (ret.Status == 1)
                            {
                                await ConsoleMessage("Tarefa pulada com sucesso", 1);
                            }
                            else
                            {
                                await ConsoleMessage("Erro ao pular tarefa", 1);
                            }
                        }
                        else
                        {
                            if (Contas[Index].Block)
                            {
                                Contas[Index].Block = false;
                                Contas[Index].RemoverBlock();
                            }
                            await SetBorder(2);
                            await ConsoleMessage("Tarefa realizada com sucesso", 2);
                            var ret = await ConfirmarAcao(Contas[Index].Username);
                            if (ret.Status == -1)
                            {
                                LogMessage($"Erro ao confirmar a tarefa");
                            }
                            if (ret.Status == 1)
                            {
                                k++;
                                if (task.Response.ToLower() == "seguir")
                                {
                                    await AddTarefa(0, i);
                                    await AddSaldo(0.007);
                                }
                                else
                                {
                                    await AddTarefa(1, i);
                                    await AddSaldo(0.003);
                                }
                            }
                            else
                            {
                                await ConsoleMessage($"Não foi possivel confirmar a tarefa pelo seguinte motivo: {ret.Response}", 4);
                            }
                        }
                    }
                    else
                    {
                        if (Global.Trocar && Contas.Count > 1)
                        {
                            await ConsoleMessage("Não foi localizada tarefa para realizar, indo para proxima conta", 1);
                            sair = true;
                        }
                        else
                        {
                            await ConsoleMessage("Não foi possivel localizar tarefa para realizar", 1);
                        }
                    }
                    if (sair == false)
                    {
                        if (task.Status == 1 || task.Status == 2)
                        {
                            await SetBorder(6);
                            var delay = r.Next(Convert.ToInt32(Global.Delay1), Convert.ToInt32(Global.Delay2));
                            await ConsoleMessage($"Aguardando {delay} segundos para continuar", 6);
                            try
                            {
                                if (Porcent(150))
                                {
                                    Driver.FindElement(By.XPath("//img[@alt='Instagram']/ancestor::a[1]")).Click();
                                }
                                else
                                {
                                    Driver.FindElement(By.XPath("//a[@href='/']")).Click();
                                }
                            }
                            catch { }
                            delay /= 2;
                            await Task.Delay(TimeSpan.FromSeconds(delay));
                        }
                        else
                        {
                            await SetBorder(6);
                            await ConsoleMessage($"Aguardando {3} segundos para continuar", 6);
                            await Task.Delay(TimeSpan.FromSeconds(3));
                        }
                    }
                }
                await ConsoleMessage($"Indo para a proxima conta", 1);
                if (Contas.Count > 1)
                    InstagramGNI[i].isLogged = false;
                return;
            }
            catch (Exception err)
            {
                LogMessage($"Erro RodarConta: {err.Message}");
                await ConsoleMessage("Erro ao rodar o bot, entre em contato com o suporte", 5);
                if (Contas.Count > 1)
                    InstagramGNI[i].isLogged = false;
                return;
            }
        }

        #endregion

        #region Realizar Tarefa

        private async Task<Retorno> RealizarTarefa(Retorno task, int index)
        {
            Retorno ret = new Retorno { Response = "", Status = 0 };
            try
            {
                if (task.Response.ToLower() == "seguir")
                {
                    string alvo = "";
                    var array = Driver.Url.Split("/");
                    if (array[^1] == "")
                        alvo = array[^2];
                    else
                        alvo = array[^1];
                    var seguir = await Seguir(alvo);
                    if (seguir.Status == 1)
                    {
                        ret.Status = 1;
                        ret.Response = "Sucesso ao realizar a tarefa";
                        if (Driver.WindowHandles.Count > 1)
                        {
                            Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                            await Task.Delay(478);
                            Driver.Close();
                            Driver.SwitchTo().Window(Driver.WindowHandles.First());
                            await Task.Delay(478);
                        }
                        return ret;
                    }
                    else
                    {
                        switch (seguir.Status)
                        {
                            case -1:
                                ret.Status = -1;
                                ret.Response = "Pagina do instagram não carrega";
                                if (Driver.WindowHandles.Count > 1)
                                {
                                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                    await Task.Delay(478);
                                    Driver.Close();
                                    Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                    await Task.Delay(478);
                                }
                                return ret;
                            case 0:
                                ret.Status = 0;
                                ret.Response = "Bloqueio de SMS";
                                Contas[Index].Challeng = true;
                                Contas[Index].AdicionarChallenge();
                                Challenges.Add(Contas[Index].Username);
                                await SetBorder(4);
                                if (Driver.WindowHandles.Count > 1)
                                {
                                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                    await Task.Delay(478);
                                    Driver.Close();
                                    Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                    await Task.Delay(478);
                                }
                                return ret;
                            case 2:
                                ret.Status = 2;
                                ret.Response = "Bloqueio: Confirmar Login";
                                Contas[Index].Challeng = true;
                                Contas[Index].AdicionarChallenge();
                                Challenges.Add(Contas[Index].Username);
                                await SetBorder(4);
                                if (Driver.WindowHandles.Count > 1)
                                {
                                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                    await Task.Delay(478);
                                    Driver.Close();
                                    Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                    await Task.Delay(478);
                                }
                                return ret;
                            case 3:
                                ret.Status = 3;
                                ret.Response = "Ja seguia o perfil";
                                if (Driver.WindowHandles.Count > 1)
                                {
                                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                    await Task.Delay(478);
                                    Driver.Close();
                                    Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                    await Task.Delay(478);
                                }
                                return ret;
                            case 4:
                                ret.Status = 4;
                                ret.Response = "Bloqueio temporário";
                                Contas[Index].Block = true;
                                Contas[Index].AdicionarBlock();
                                Bloqueios.Add(new Block { Inicio = DateTime.Now, Minutes = Global.Timer_Block, Username = Contas[Index].Username });
                                await SetBorder(3);
                                if (Driver.WindowHandles.Count > 1)
                                {
                                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                    await Task.Delay(478);
                                    Driver.Close();
                                    Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                    await Task.Delay(478);
                                }
                                return ret;
                            case 5:
                                ret.Status = 5;
                                ret.Response = "Erro ao realizar tarefa";
                                if (Driver.WindowHandles.Count > 1)
                                {
                                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                    await Task.Delay(478);
                                    Driver.Close();
                                    Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                    await Task.Delay(478);
                                }
                                return ret;
                            default:
                                ret.Status = -1;
                                ret.Response = "Erro ao realizar a tarefa";
                                if (Driver.WindowHandles.Count > 1)
                                {
                                    Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                    await Task.Delay(478);
                                    Driver.Close();
                                    Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                    await Task.Delay(478);
                                }
                                return ret;
                        }
                    }
                }
                else
                {
                    if (task.Response.ToLower() == "curtir")
                    {
                        string link = Driver.Url;
                        var cur = await Curtir(link);
                        if (cur.Status == 1)
                        {
                            ret.Status = 1;
                            ret.Response = "Sucesso ao realizar a tarefa";
                            if (Driver.WindowHandles.Count > 1)
                            {
                                Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                await Task.Delay(478);
                                Driver.Close();
                                Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                await Task.Delay(478);
                            }
                            return ret;
                        }
                        else
                        {
                            switch (cur.Status)
                            {
                                case -1:
                                    ret.Status = -1;
                                    ret.Response = "Pagina do instagram não carrega";
                                    if (Driver.WindowHandles.Count > 1)
                                    {
                                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                        await Task.Delay(478);
                                        Driver.Close();
                                        Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                        await Task.Delay(478);
                                    }
                                    return ret;
                                case 0:
                                    ret.Status = 0;
                                    ret.Response = "Bloqueio de SMS";
                                    Contas[Index].Challeng = true;
                                    Contas[Index].AdicionarChallenge();
                                    Challenges.Add(Contas[Index].Username);
                                    await SetBorder(4);
                                    if (Driver.WindowHandles.Count > 1)
                                    {
                                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                        await Task.Delay(478);
                                        Driver.Close();
                                        Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                        await Task.Delay(478);
                                    }
                                    return ret;
                                case 2:
                                    ret.Status = 2;
                                    ret.Response = "Bloqueio: Confirmar Login";
                                    Contas[Index].Challeng = true;
                                    Contas[Index].AdicionarChallenge();
                                    Challenges.Add(Contas[Index].Username);
                                    await SetBorder(4);
                                    if (Driver.WindowHandles.Count > 1)
                                    {
                                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                        await Task.Delay(478);
                                        Driver.Close();
                                        Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                        await Task.Delay(478);
                                    }
                                    return ret;
                                case 3:
                                    ret.Status = 3;
                                    ret.Response = "Ja seguia o perfil";
                                    if (Driver.WindowHandles.Count > 1)
                                    {
                                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                        await Task.Delay(478);
                                        Driver.Close();
                                        Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                        await Task.Delay(478);
                                    }
                                    return ret;
                                case 4:
                                    ret.Status = 4;
                                    ret.Response = "Bloqueio temporário";
                                    Contas[Index].Block = true;
                                    Contas[Index].AdicionarBlock();
                                    Bloqueios.Add(new Block { Inicio = DateTime.Now, Minutes = Global.Timer_Block, Username = Contas[Index].Username });
                                    await SetBorder(3);
                                    if (Driver.WindowHandles.Count > 1)
                                    {
                                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                        await Task.Delay(478);
                                        Driver.Close();
                                        Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                        await Task.Delay(478);
                                    }
                                    return ret;
                                case 5:
                                    ret.Status = 5;
                                    ret.Response = "Erro ao realizar tarefa";
                                    if (Driver.WindowHandles.Count > 1)
                                    {
                                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                        await Task.Delay(478);
                                        Driver.Close();
                                        Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                        await Task.Delay(478);
                                    }
                                    return ret;
                                default:
                                    ret.Status = -1;
                                    ret.Response = "Erro ao realizar a tarefa";
                                    if (Driver.WindowHandles.Count > 1)
                                    {
                                        Driver.SwitchTo().Window(Driver.WindowHandles.Last());
                                        await Task.Delay(478);
                                        Driver.Close();
                                        Driver.SwitchTo().Window(Driver.WindowHandles.First());
                                        await Task.Delay(478);
                                    }
                                    return ret;
                            }
                        }
                    }
                    else
                    {
                        ret.Status = -1;
                        ret.Response = $"O tipo de tarefa '{task.Response}' não está configurada";
                        return ret;
                    }
                }
            }
            catch (Exception err)
            {
                ret.Status = -1;
                ret.Response = $"O seguinte erro aconteceu: {err.Message}";
            }
            return ret;
        }

        #endregion
    }
}
