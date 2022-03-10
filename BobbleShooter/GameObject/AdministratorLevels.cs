using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;

namespace BobbleShooter
{
    public class AdministratorLevels
    {
        public List<Level> Levels { get; private set; }

        public AdministratorLevels()
        {

            loadNewGame();
        }

        private string readFile(string path)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            StringBuilder sb = new StringBuilder();
            StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(path));
            string line = reader.ReadLine();
            // Se deja todo el archivo como una sola línea
            while (line != null)
            {
                sb.Append(line);
                sb.Append("\n");
                line = reader.ReadLine();
            }
            reader.Close();
            return sb.ToString();
        }

        public void loadNewGame()
        {
            Levels = new List<Level>();

            String data = readFile("BobbleShooter.GameObject.gameLevels.txt");
            String[] levels = data.Split(new char[] { '-' }); // split level with '-'
            Int32 row = 0;
            List<Bubble> dataLevel = new List<Bubble>();
            // search each level
            foreach (String Level in levels)
            {

                // search each lines
                String[] lines = Level.Split(new char[] { '\n' });
                foreach (String line in lines)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        int column = 0;
                        // split buble with ' '
                        foreach (String item in line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            // add bubble except 9
                            if (item != "9")
                            {
                                Bubble b = new Bubble(Vector2.Zero, bubbleShooter.colors[Int32.Parse(item)]);
                                b.pos_bubbleBox = new Vector2(column, row);
                                dataLevel.Add(b);
                            }
                            column++;
                        }
                        row++;
                    }
                }

                // add new level
                Levels.Add(new Level(dataLevel));
                dataLevel = new List<Bubble>();
                row = 0;
            }
        }
    }
}
