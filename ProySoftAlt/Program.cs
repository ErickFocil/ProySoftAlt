// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

namespace ProySoftAlt{
    internal class program {
        static void Main(string[] args) {
            Stopwatch swTodo = new Stopwatch();
            Stopwatch swArchivo = new Stopwatch();
            Stopwatch swCrear = new Stopwatch();
            swTodo.Start();

            //Log
            String logD = @"C:\CS13309\a7_2703119.txt";
            FileStream log = File.Create(logD);
            log.Close();
            StreamWriter swLog = new StreamWriter(logD);

            DirectoryInfo di = new DirectoryInfo(@"C:\CS13309\Files");

            foreach (var file in di.GetFiles("*.html"))
            {
                swArchivo.Start();

                QuitarEtiquetas(@"C:\CS13309\FilesSinEtiquetas\", file);
                SepararYOrdenarPalabras(@"C:\CS13309\FilesSinEtiquetas\", @"C:\CS13309\FilesLetras\", file.Name);

                // Comentar cuando se realizen pruebas
                CrearTokens(@"C:\CS13309\FilesLetras\", @"C:\CS13309\Tokens\", file.Name);

                //Terminar contador
                swArchivo.Stop();
                TimeSpan swArchivoE = swArchivo.Elapsed;
                String logimprimir = file.FullName + " -> " + swArchivoE.TotalSeconds;
                Console.WriteLine(logimprimir);
                swLog.WriteLine(logimprimir);
                swArchivo.Reset();
            }
            
            /** Trabajando solo con 4 elementos **/
            // Tokenizar
            swCrear.Start();
            //List<string> palabras = new List<string>();
            string[] vs = new string[] {
                "simple.html",
                "medium.html",
                "hard.html",
                "049.html"};
            // Loop que iría en el loop principal
            foreach (String t in vs)
            {
                // Comentar cuando se prueben con todos los archivos
                //CrearTokens(@"C:\CS13309\FilesLetras\", @"C:\CS13309\Tokens\", t);
            }

            UnificarTokens(@"C:\CS13309\Tokens\", @"C:\CS13309\", "Tokens.txt", "Posting.txt");

            swCrear.Stop();
            double tBCrear = swCrear.Elapsed.TotalSeconds;


            swTodo.Stop();
            TimeSpan swTodoE = swTodo.Elapsed;
            string mCrear = "Tiempo total de creación de todos los Tokens: " + tBCrear;
            string mTodo = "Tiempo total de ejecución: " + swTodoE.TotalSeconds;
            //swLog.WriteLine(mCrear);
            swLog.WriteLine(mTodo);
            //Console.WriteLine(mCrear);
            Console.WriteLine(mTodo);
            swLog.Close();
        }

        static void QuitarEtiquetas(string dirN, FileInfo file)
        {
            //Directorio
            //String direcO = @"C:\CS13309\FilesSinEtiquetas\" + file.Name;
            string direcO = dirN + file.Name;

            //Quitar Etiquetas
            Stream sr = new FileStream(file.FullName, FileMode.Open);
            FileStream fs = File.Create(direcO); fs.Close();
            StreamWriter sw = new StreamWriter(direcO);

            bool etiqueta = false;
            while (true)
            {
                int dato = sr.ReadByte();
                if (dato < 0) break;

                if (Convert.ToChar(dato).Equals('<')) etiqueta = true;

                if (!etiqueta) sw.WriteAsync(Convert.ToChar(dato)).Wait();

                if (Convert.ToChar(dato).Equals('>')) etiqueta = false;
            }
            sw.Close();
            sr.Close();
        }
        
        static void SepararYOrdenarPalabras(string dirO, string dirN, string fileName)
        {
            // Extraer cada palabra
            List<String> palabras = new List<String>();
            string direcPO = dirO + fileName;
            Stream sr = new FileStream(direcPO, FileMode.Open);

            char cA = ' ';
            String palabra = "";
            while (true)
            {
                int dato = sr.ReadByte();
                if (dato < 0) break;

                char c = Convert.ToChar(dato);
                if (Char.IsLetter(c)) palabra += c.ToString();
                else if (!Char.IsLetter(c) && Char.IsLetter(cA))
                {
                    palabras.Add(palabra);
                    palabra = "";
                }
                cA = c;
            }
            sr.Close();

            // Ordenar, Crear y Escribir en nuevo archivo
            string direcPN = dirN + fileName;
            FileStream fs = File.Create(direcPN); fs.Close();
            StreamWriter sw = new StreamWriter(direcPN);

            palabras.Sort((x, y) => String.Compare(x, y));

            foreach (String p in palabras)
            {
                sw.WriteLine(p.ToLower());
            }
            sw.Close();
        }

        static void CrearTokens(string dirO, string dirN, string fileName)
        {
            List<String> palabras = new List<String>();
            string direcTO = dirO + fileName;
            StreamReader sr = new StreamReader(direcTO);
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                //if (!String.IsNullOrWhiteSpace(line)) palabras.Add((String)line);
                palabras.Add((String)line);
            }
            sr.Close();

            var palabrasO = palabras.GroupBy(x => x)
              .Where(g => g.Count() > 0)
              .Select(y => new { Palabra = y.Key, Cantidad = y.Count() })
              .ToList();

            palabrasO.Sort((x, y) => y.Cantidad.CompareTo(x.Cantidad));

            // Crear archivo
            string direcTN = dirN + fileName;
            FileStream fs = File.Create(direcTN); fs.Close();
            StreamWriter sw = new StreamWriter(direcTN);

            foreach (var p in palabrasO)
            {
                sw.WriteLine(p.Palabra + " " + p.Cantidad);
            }
            sw.Close();
        }

        static void UnificarTokens(string dirO, string dirN, string fileName, string fnPosting)
        {
            List<NumRPalabras> palabras = new List<NumRPalabras>();
            DirectoryInfo di = new DirectoryInfo(dirO);

            foreach (FileInfo fi in di.GetFiles("*.html"))
            {
                StreamReader sr = new StreamReader(fi.FullName);
                string? line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] split = line.Split(" ");
                    palabras.Add(new NumRPalabras(split[0], Int32.Parse(split[1])) { archivo = fi.Name});
                }
                sr.Close();
            }

            var unido = palabras.GroupBy(x => x.p)
              .Where(g => g.Count() > 0)
              .Select(y => new { Palabra = y.Key, sum = y.Count() })
              .ToList();

            palabras.Sort((x,y) => x.p.CompareTo(y.p));


            
            // Crear archivo
            string direcTN = dirN + fileName;
            string direcTNP = dirN + fnPosting;
            FileStream fs = File.Create(direcTN); fs.Close();
            fs = File.Create(direcTNP); fs.Close();
            StreamWriter sw = new StreamWriter(direcTN);
            StreamWriter swP = new StreamWriter(direcTNP);

            string prev = "";
            int c = 1, acum = 0, cont = 0, contI = 0;
            foreach (NumRPalabras p in palabras)
            {
                swP.WriteLine(p.archivo + ";" + p.id);

                if (prev.Equals("")) {
                    contI = 0;
                    prev = p.p;
                    acum = p.id;
                } else if (p.p.Equals(prev)) {
                    c++;
                    acum += p.id;
                } else {
                    sw.WriteLine(prev + ";" + c + ";" + contI);
                    contI = cont;
                    c = 1;
                    acum = p.id;
                    prev = p.p;
                }

                cont++;
            }
            sw.Write(prev + ";" + c + ";" + cont);  //Ultimo archivo
            swP.Close();
            sw.Close();
        }
        
        static void OrdenarTokensR(List<NumRPalabras> palabras, string dir, string fileName)
        {
            //var temp = palabras.ToList(p);
        }
    }

    public class NumRPalabras
    {
        public string p;
        public int id;
        public string archivo;
        public NumRPalabras(string p, int id) { this.p = p; this.id = id; }
    }
}