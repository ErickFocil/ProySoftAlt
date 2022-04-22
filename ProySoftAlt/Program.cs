// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using System.Collections;

namespace ProySoftAlt{
    internal class program {
        static void Main(string[] args) {

            string[] directorios =
            {
                @"C:\CS13309\FilesSinEtiquetas\",
                @"C:\CS13309\FilesLetras\",
                @"C:\CS13309\Tokens\"
            };
            LimpiarDirectorios(directorios);

            Stopwatch swTodo = new Stopwatch();
            Stopwatch swArchivo = new Stopwatch();
            swTodo.Start();

            //Log
            String logD = @"C:\CS13309\aE_2703119.txt";
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
            // Usando solo éste paso se omite los siguientes
            // UnificarTokens(@"C:\CS13309\Tokens\", @"C:\CS13309\", "Tokens.txt", "Posting.txt", @"C:\CS13309\stoplist.txt");

            // Juntar Tokens
            swArchivo.Start();
            List<NumRPalabras> palabrasDic = UnificarTokens(@"C:\CS13309\Tokens\");

            //Ordenar - en desuso ya que en filtros se ordena
            //palabras.Sort((x,y) => x.p.CompareTo(y.p));

            // Ejecutar filtros
            palabrasDic = Filtrar(palabrasDic, @"C:\CS13309\stoplist.txt");

            // Archivo para contador
            TimeSpan[] tSCArchivos = new TimeSpan[2];

            // Crear Archivos
            Hashtable htDic = CrearHTDic(palabrasDic);
            CrearArchivoTokens(palabrasDic, htDic, @"C:\CS13309\", "Tokens.txt");
            swArchivo.Stop();
            tSCArchivos[0] = swArchivo.Elapsed;

            swArchivo.Start();
            CrearArchivoPosting(palabrasDic, htDic, @"C:\CS13309\", "Posting.txt");
            swArchivo.Stop();
            tSCArchivos[1] = swArchivo.Elapsed;


            swTodo.Stop();
            TimeSpan swTodoE = swTodo.Elapsed;
            string mTodo = "Milisegundos de creación de Token: " + tSCArchivos[0].TotalMilliseconds + "\n" +
                "Milisegundos de creación Tokens y de Posting: " + tSCArchivos[1].TotalMilliseconds + "\n" +
                "Tiempo total de ejecución: " + swTodoE.TotalSeconds;
            swLog.Write(mTodo);
            Console.Write(mTodo);
            swLog.Close();
        }

        static void AnalizarArgs(string[] args)
        {

        }

        static void LimpiarDirectorios(string[] dic)
        {
            foreach (string d in dic)
            {
                DirectoryInfo di = new DirectoryInfo(d);

                foreach (FileInfo file in di.GetFiles())
                    file.Delete();
                foreach (DirectoryInfo dir in di.GetDirectories())
                    dir.Delete(true);
            }
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

        // En Desuso
        /**
        static void UnificarTokensO(string dirO, string dirN, string fnToken, string fnPosting, string stoplist)
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

            //palabras.Sort((x,y) => x.p.CompareTo(y.p));

            // Ejecutar filtros
            palabras = Filtrar(palabras,stoplist);
            
            // Crear archivo
            string direcTN = dirN + fnToken;
            string direcTNP = dirN + fnPosting;
            FileStream fs = File.Create(direcTN); fs.Close();
            fs = File.Create(direcTNP); fs.Close();
            StreamWriter sw = new StreamWriter(direcTN);
            StreamWriter swP = new StreamWriter(direcTNP);

            Hashtable htPosting = new Hashtable();
            Hashtable htDic = new Hashtable();

            string prev = "";
            int c = 1, acum = 0, cont = 0, contI = 0;
            foreach (NumRPalabras p in palabras)
            {
                //swP.WriteLine(p.archivo + ";" + p.id);
                // Codigo funcional con List en vez del Hash
                //if (prev.Equals(""))
                //{
                //    contI = 0;
                //    prev = p.p;
                //    acum = p.id;
                //}
                //else if (p.p.Equals(prev))
                //{
                //    c++;
                //    acum += p.id;
                //}
                //else
                //{
                //    sw.WriteLine(prev + ";" + c + ";" + contI);
                //    contI = cont;
                //    c = 1;
                //    acum = p.id;
                //    prev = p.p;
                //}
                //
                htPosting.Add(cont, p);
                if (htDic.ContainsKey(p.p)) ((NumRPalabras)htDic[p.p]).id++;
                else htDic.Add(p.p, new NumRPalabras(p.p, 1) { c = cont});

                cont++;
            }
            // Va conjunto al del código funcional de List en vez del Hash
            //sw.Write(prev + ";" + c + ";" + cont);  //Ultimo archivo

            // Llenar archivo Diccionario de Tokens
            // El Hashtable necesita ser pasado a un List por los objetos
            List<NumRPalabras> dicT = new List<NumRPalabras>();
            foreach(string d in htDic.Keys)
                dicT.Add(new NumRPalabras(d, ((NumRPalabras)htDic[d]).id) { c = ((NumRPalabras)htDic[d]).c });

            dicT.Sort((x, y) => x.p.CompareTo(y.p));
            foreach(NumRPalabras d in dicT) sw.WriteLine(d.p + ";" + d.id + ";" + d.c);
            sw.Close();

            // Llenar archivo de Posting
            List<NumRPalabras> listP = new List<NumRPalabras>();
            foreach(int p in htPosting.Keys)
                listP.Add((NumRPalabras)htPosting[p]);

            foreach (NumRPalabras d in palabras)
                swP.WriteLine(d.archivo + ";" + tfidf(d.id, ((NumRPalabras)htDic[d.p]).id).ToString("N4"));
                //swP.WriteLine(d.archivo + ";" + d.id);
            swP.Close();
        }
        **/
        static List<NumRPalabras> UnificarTokens(string dirO)
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
                    palabras.Add(new NumRPalabras(split[0], Int32.Parse(split[1])) { archivo = fi.Name });
                }
                sr.Close();
            }
            return palabras;
        }
        
        // En Desuso
        /**
        static void CrearArchivos(List<NumRPalabras> palabras, string dirN, string fnToken, string fnPosting)
        {
            // Crear archivo
            string direcTN = dirN + fnToken;
            string direcTNP = dirN + fnPosting;
            FileStream fs = File.Create(direcTN); fs.Close();
            fs = File.Create(direcTNP); fs.Close();
            StreamWriter sw = new StreamWriter(direcTN);
            StreamWriter swP = new StreamWriter(direcTNP);

            Hashtable htPosting = new Hashtable();
            Hashtable htDic = new Hashtable();

            int cont = 0;
            foreach (NumRPalabras p in palabras)
            {
                htPosting.Add(cont, p);
                if (htDic.ContainsKey(p.p)) ((NumRPalabras)htDic[p.p]).id++;
                else htDic.Add(p.p, new NumRPalabras(p.p, 1) { c = cont });

                cont++;
            }

            // Llenar archivo Diccionario de Tokens
            // El Hashtable necesita ser pasado a un List por los objetos
            List<NumRPalabras> dicT = new List<NumRPalabras>();
            foreach (string d in htDic.Keys)
                dicT.Add(new NumRPalabras(d, ((NumRPalabras)htDic[d]).id) { c = ((NumRPalabras)htDic[d]).c });

            dicT.Sort((x, y) => x.p.CompareTo(y.p));
            foreach (NumRPalabras d in dicT) sw.WriteLine(d.p + ";" + d.id + ";" + d.c);
            sw.Close();

            // Llenar archivo de Posting
            List<NumRPalabras> listP = new List<NumRPalabras>();
            foreach (int p in htPosting.Keys)
                listP.Add((NumRPalabras)htPosting[p]);

            foreach (NumRPalabras d in palabras)
                swP.WriteLine(d.archivo + ";" + tfidf(d.id, ((NumRPalabras)htDic[d.p]).id).ToString("N4"));
            //swP.WriteLine(d.archivo + ";" + d.id);
            swP.Close();
        }
        **/
        static Hashtable CrearHTDic(List<NumRPalabras> palabras)
        {
            Hashtable htDic = new Hashtable();

            int cont = 0;
            foreach (NumRPalabras p in palabras)
            {
                if (htDic.ContainsKey(p.p)) ((NumRPalabras)htDic[p.p]).id++;
                else htDic.Add(p.p, new NumRPalabras(p.p, 1) { c = cont });

                cont++;
            }

            return htDic;
        }

        static void CrearArchivoTokens(List<NumRPalabras> palabras, Hashtable htDic, string dirN, string fnToken)
        {
            // Crear archivo
            string direcTN = dirN + fnToken;
            FileStream fs = File.Create(direcTN); fs.Close();
            StreamWriter sw = new StreamWriter(direcTN);

            // Llenar archivo Diccionario de Tokens
            // El Hashtable necesita ser pasado a un List por los objetos
            List<NumRPalabras> dicT = new List<NumRPalabras>();
            foreach (string d in htDic.Keys)
                dicT.Add(new NumRPalabras(d, ((NumRPalabras)htDic[d]).id) { c = ((NumRPalabras)htDic[d]).c });

            dicT.Sort((x, y) => x.p.CompareTo(y.p));
            foreach (NumRPalabras d in dicT) sw.WriteLine(d.p + ";" + d.id + ";" + d.c);
            sw.Close();
        }

        static void CrearArchivoPosting(List<NumRPalabras> palabras, Hashtable htDic, string dirN, string fnPosting)
        {
            // Crear archivo
            string direcTNP = dirN + fnPosting;
            FileStream fs = File.Create(direcTNP); fs.Close();
            StreamWriter swP = new StreamWriter(direcTNP);

            foreach (NumRPalabras d in palabras)
                swP.WriteLine(d.archivo + ";" + tfidf(d.id, ((NumRPalabras)htDic[d.p]).id).ToString("N4"));
            //swP.WriteLine(d.archivo + ";" + d.id);
            swP.Close();
        }

        static List<NumRPalabras> Filtrar(List<NumRPalabras> palabras, string stoplist)
        {
            // Ordeno las palabras
            palabras.Sort((x, y) => x.p.CompareTo(y.p));
            
            // Inicializar las palabras a omitir
            StreamReader srSL = new StreamReader(stoplist);
            List<string> palabrasSL = new List<string>();
            var line = "";
            while ((line = srSL.ReadLine()) != null) palabrasSL.Add((String)line);
            srSL.Close();
            //foreach(string p in palabrasSL) Console.WriteLine("<" + p + ">");

            List<NumRPalabras> palabrasT = new List<NumRPalabras>();

            // Fitlrar palabras de un solo caracter
            foreach (NumRPalabras p in palabras)
                if (p.p.Length > 1) palabrasT.Add(p);

            // Asigno la lista temporal al principal y reseteo la lista temporal
            palabras = palabrasT;
            palabrasT = new List<NumRPalabras>();

            // Filtrar por palabras a excluir
            foreach (NumRPalabras p in palabras)
            {
                bool filtrar = false;
                foreach (string ps in palabrasSL)
                {
                    if (p.p.Equals(ps))
                    {
                        filtrar = true;
                        break;
                    }
                }
                if (!filtrar) palabrasT.Add(p);
            }
            // Asigno la lista temporal al principal y reseteo la lista temporal
            palabras = palabrasT;
            palabrasT = new List<NumRPalabras>();

            // Contar cantidad de repeticiones de cada palabra en todos los archivos
            Hashtable htSum = new Hashtable();
            int lim = 5;
            foreach (NumRPalabras p in palabras)
            {
                if (htSum.ContainsKey(p.p)) htSum[p.p] = (int)htSum[p.p] + p.id;
                else htSum.Add(p.p, p.id);
            }
            Hashtable htDrop = new Hashtable();
            foreach (string k in htSum.Keys)
                if((int)htSum[k] < lim) htDrop.Add(k, htSum[k]);

            // Filtrar según las palabras seleccionadas
            foreach (NumRPalabras p in palabras)
            {
                bool filtrar = false;
                if(htDrop.ContainsKey(p.p)) filtrar = true;
                if (!filtrar) palabrasT.Add(p);
            }

            // Asigno la lista temporal al principal y reseteo la lista temporal
            palabras = palabrasT;
            //palabrasT = new List<NumRPalabras>();

            return palabras;
        }

        static double tfidf(int rep, int total)
        {
            double res = 0.0000;
            res = ((double)rep * 100) / (double)total;
            return res;
        }
        
    }

    public class NumRPalabras
    {
        public string p;
        public int id;
        public int c;
        public string archivo;
        public NumRPalabras(string p, int id) { this.p = p; this.id = id; }
    }
}