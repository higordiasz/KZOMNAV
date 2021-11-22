using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KZOMNAV.Models.Waits
{
    class Wait
    {
        public int TYPE { get; set; }
        public string VALUE { get; set; }
    }

    public static class WaitTypes
    {
        public static int TYPE_ID { get { return 1; } }
        public static int TYPE_NAME { get { return 2; } }
        public static int TYPE_XPATH { get { return 3; } }
        public static int TYPE_CSS { get { return 4; } }
        public static int TYPE_CLASSNAME { get { return 5; } }
        public static int TYPE_LINKTEXT { get { return 6; } }
    }

    static class Waits
    {
        /// <summary>
        /// Espera que alguma parte da pagina carregar
        /// </summary>
        /// <param name="list">Lista com os wait</param>
        /// <param name="qtd">Quantidade: Equivale a 5s cada</param>
        /// <returns>Retorna o index da lista</returns>
        static public async Task<int> Wait (List<Wait> list, int qtd, IWebDriver Driver)
        {
            try
            {
                int i = 0;
                while (i < qtd)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        try
                        {
                            if (list[j].TYPE == 1)
                            {
                                Driver.FindElement(By.Id(list[j].VALUE));
                                return j;
                            }
                            else
                            {
                                if (list[j].TYPE == 2)
                                {
                                    Driver.FindElement(By.Name(list[j].VALUE));
                                    return j;
                                }
                                else
                                {
                                    if (list[j].TYPE == 3)
                                    {
                                        Driver.FindElement(By.XPath(list[j].VALUE));
                                        return j;
                                    }
                                    else
                                    {
                                        if (list[j].TYPE == 4)
                                        {
                                            Driver.FindElement(By.CssSelector(list[j].VALUE));
                                            return j;
                                        }
                                        else
                                        {
                                            if (list[j].TYPE == 5)
                                            {
                                                Driver.FindElement(By.ClassName(list[j].VALUE));
                                                return j;
                                            }
                                            else
                                            {
                                                if (list[j].TYPE == 6)
                                                {
                                                    Driver.FindElement(By.LinkText(list[j].VALUE));
                                                    return j;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        } catch
                        { }
                    }
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    i++;
                }
                return -1;
            } catch
            {
                return -1;
            }
        }
    }
}
