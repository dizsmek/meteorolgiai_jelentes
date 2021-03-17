using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// feladat: http://informatika.fazekas.hu/wp-content/uploads/2015/10/Meteorológiai-jelentés.pdf
namespace metjelentes
{
    class Tavirat
    {
        public string Telepules { get; set; }
        public string Ido { get; set; }
        public string SzelInfo { get; set; }
        public string Szelirany { get; set; }
        public string Szelerosseg { get; set; }
        public string Homerseklet { get; set; }

        public string IdoFormalva() => $"{Ido.Substring(0, 2)}:{Ido.Substring(2)}";

        public Tavirat(string telepules, string ido, string szelinfo, string homerseklet)
        {
            string szelirany = szelinfo.Substring(0, 3);
            string szelerosseg = szelinfo.Substring(3);

            Telepules = telepules;
            Ido = ido;
            SzelInfo = szelinfo;
            Szelirany = szelirany;
            Szelerosseg = szelerosseg;
            Homerseklet = homerseklet;
        }
    }

    class MainClass
    {
        static public List<Tavirat> taviratok = new List<Tavirat>();
        static public IEnumerable<string> telepulesek = taviratok.Select(x => x.Telepules).Distinct();

        public static void Main(string[] args)
        {


            #region 1. feladat
            StreamReader file = new StreamReader(new FileStream("tavirathu13.txt", FileMode.Open));

            while (!file.EndOfStream)
            {
                string line = file.ReadLine();
                string[] values = line.Split(' ');

                Tavirat tavirat = new Tavirat(
                    telepules: values[0],
                    ido: values[1],
                    szelinfo: values[2],
                    homerseklet: values[3]
                );

                taviratok.Add(tavirat);
            }

            file.Close();
            #endregion

            // 2. feladat
            Feladat2();

            // 3. feladat
            Feladat3();

            // 4. feladat
            Feladat4();

            // 5. feladat
            Feladat5();

            // 6. feladat
            Feladat6();

            Console.ReadKey();
            Console.Clear();
        }

        public static void Feladat2()
        {
            Console.WriteLine("2. feladat: \n");

            Console.Write("Adja meg egy varos kodjat: ");
            string varosKod = Console.ReadLine();

            // Rendezze az adott varos meresi adatait az ido szerint novekvo sorrendbe es kerje el az utolsot
            var tavirat = taviratok
                .Where(x => x.Telepules == varosKod)
                .OrderBy(x => Convert.ToInt32(x.Ido))
                .Last();

            string ora = tavirat.Ido.Substring(0, 2);
            string perc = tavirat.Ido.Substring(2);

            Console.WriteLine($"Az utolso meresi adat {tavirat.IdoFormalva()}-kor erkezett");
        }

        public static void Feladat3()
        {
            Console.WriteLine("3. feladat: \n");

            // taviratok homerseklet szerint novekvo sorrendbe rendezve
            IEnumerable<Tavirat> homersekletSzerintSorba = taviratok.OrderBy(x => x.Homerseklet);

            Tavirat leghidegebbMeres = homersekletSzerintSorba.First();
            Tavirat legmelegebbMeres = homersekletSzerintSorba.Last();

            Console.WriteLine(
                "A legalacsonyabb meres {0} fok volt {1} varosban, {2}-kor.",
                leghidegebbMeres.Homerseklet,
                leghidegebbMeres.Telepules,
                leghidegebbMeres.IdoFormalva()
            );

            Console.WriteLine(
                "A legmagasabb meres {0} fok volt {1} varosban, {2}-kor.",
                legmelegebbMeres.Homerseklet,
                legmelegebbMeres.Telepules,
                legmelegebbMeres.IdoFormalva()
            );
        }

        public static void Feladat4()
        {
            Console.WriteLine("4. feladat: \n");

            IEnumerable<Tavirat> szelcsendesek = taviratok.Where(x => x.SzelInfo == "00000");

            if (szelcsendesek.Count() == 0)
            {
                Console.WriteLine("Nem volt szélcsend a mérések idején.");
                return;
            }

            foreach (Tavirat tavirat in szelcsendesek)
            {
                Console.WriteLine($"{tavirat.Telepules}: {tavirat.IdoFormalva()}");
            }
        }

        public static void Feladat5()
        {
            Console.WriteLine("5. feladat: \n");

            string[] orak = { "01", "07", "13", "19" };

            string atlagHomerseklet(string telepules)
            {
                IEnumerable<Tavirat> relevansak = taviratok
                    .Where(x => x.Telepules == telepules)
                    .Where(x => Array.IndexOf(orak, x.Ido.Substring(0, 2)) > -1);

                return relevansak.Count() != 0 ?
                    Convert.ToString(
                        Math.Round(
                            relevansak
                                .Average(x => Convert.ToInt32(x.Homerseklet)), 0)
                    ) : "NA";
            }

            int ingadozas(string telepules)
            {
                IEnumerable<Tavirat> relevansak = taviratok
                    .Where(x => x.Telepules == telepules);

                int min = Convert.ToInt32(
                    taviratok
                        .Where(x => x.Telepules == telepules)
                        .OrderBy(x => x.Homerseklet)
                        .First().Homerseklet
                );

                int max = Convert.ToInt32(
                    taviratok
                        .Where(x => x.Telepules == telepules)
                        .OrderBy(x => x.Homerseklet)
                        .Last().Homerseklet
                );

                return max - min;
            }


            foreach (string telepules in telepulesek)
            {
                Console.WriteLine($"{telepules} Középhőmérséklet: {atlagHomerseklet(telepules)}; Homerseklet-ingadozas: {ingadozas(telepules)}");
            }
        }

        public static void Feladat6()
        {
            Console.WriteLine("6. feladat: \n");

            foreach (string telepules in telepulesek)
            {

                StreamWriter telepulesFile = new StreamWriter(new FileStream($"{telepules}.txt", FileMode.Create));
                telepulesFile.WriteLine(telepules);

                IEnumerable<Tavirat> telepulesTaviratai = taviratok.Where(x => x.Telepules == telepules);

                foreach (Tavirat tavirat in telepulesTaviratai)
                {

                    telepulesFile.WriteLine($"{tavirat.IdoFormalva()} {new string('#', Convert.ToInt32(tavirat.Szelerosseg))}");
                }

                telepulesFile.Close();
            }

            Console.WriteLine("A fajlok elkeszultek");
        }
    }
}
