// See https://aka.ms/new-console-template for more information
using System.Diagnostics;

namespace ProySoftAlt{
    internal class program{
        static void Main(string[] args){
            Stopwatch swTodo = new Stopwatch();
            Stopwatch swArchivo = new Stopwatch();
            Stopwatch swCrear = new Stopwatch();
            double tCrear = 0;
            swTodo.Start();
            //Log
            String logD = @"C:\CS13309\a4_2703119.txt";
            FileStream log = File.Create(logD);
            log.Close();
            StreamWriter swLog = new StreamWriter(logD);

            DirectoryInfo di = new DirectoryInfo(@"C:\CS13309\Files");
            List<String> palabras = new List<String>();
            foreach (var file in di.GetFiles("*.html"))
            {
                swArchivo.Start();
                //Directorios
                //String direcI = @"C:\CS13309\Files\" + file.Name;
                String direcO = @"C:\CS13309\FilesSinEtiquetas\" + file.Name;

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

                //Filrar palabras y ordenar alfabeticamente en archivo propio
                String direcL = @"C:\CS13309\FilesLetras\" + file.Name;
                sr = new FileStream(direcO, FileMode.Open);

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

                //Terminar contador
                swArchivo.Stop();
                TimeSpan swArchivoE = swArchivo.Elapsed;
                String logimprimir = file.FullName + " -> " + swArchivoE.TotalSeconds;
                tCrear += swArchivoE.TotalSeconds;
                Console.WriteLine(logimprimir);
                swLog.WriteLine(logimprimir);
                swArchivo.Reset();
            }

            //Crear Biblioteca con todas las palabras, ordenadas y en minusculas
            FileStream fsB = File.Create(@"C:\CS13309\Biblioteca.txt"); fsB.Close();
            StreamWriter swB = new StreamWriter(@"C:\CS13309\Biblioteca.txt");
            palabras.Sort((x, y) => String.Compare(x, y));

            foreach (String p in palabras)
            {
                swB.WriteLine(p.ToLower());
            }
            swB.Close();


            swTodo.Stop();
            TimeSpan swTodoE = swTodo.Elapsed;
            String mCrear = "Tiempo total de creación: " + tCrear;
            String mTodo = "Tiempo total de ejecución: " + swTodoE.TotalSeconds;
            swLog.WriteLine(mCrear);
            swLog.WriteLine(mTodo);
            Console.WriteLine(mCrear);
            Console.WriteLine(mTodo);
            swLog.Close();
        }
    }
}