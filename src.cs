using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace scribble
{
    class Program
    {
        static string word = "";

        static int score1 = 0, score2 = 0;
        static letter[] letters = new letter[26];
        static string[] dictionary = new string[33013];
        static char[,] board = new char[15, 15]; // for movement
        static UsingWord[] ControlWords = new UsingWord[5000];
        static char[,] control = new char[15, 15]; // for control
        static char[,] lastBoard = new char[15, 15]; // the final board of that round
        static bool isGameFinished = false;
        static bool isWord = false;
        static int round = 1;
        static int count = 0; // for the ControlWords array
        static char[] bag1 = new char[7] { ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        static char[] bag2 = new char[7] { ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        static char[] temptBag1 = new char[7] { ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        static char[] temptBag2 = new char[7] { ' ', ' ', ' ', ' ', ' ', ' ', ' ' };
        static int spx = 2;
        static int spy = 3;
        static int player = 1;
        static int counter = 0;
        static int counterOfWord = 0;
        static char[] queryBag = new char[16] { '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_' };
        static char[] returnbag = new char[7] { '_', '_', '_', '_', '_', '_', '_' };
        static string[] queryResultList = new string[10]; // We use this array for showing query results
        static Random rnd = new Random();
        static int querycontrol = 0;
        static Boolean checkround = false;
        static bool returnCheck = false;
        public static int finish = 0;

        struct letter
        {
            public char let;
            public int score;
            public int frequency;
        }

        struct UsingWord
        {
            public string word;
            public int x; // position of the first letter of the detected word
            public int y; // position of the first letter of the detected word
        }

        public static string QueryResultWriter(int index) // We write results of query
        {
            return (queryResultList[index]);
        }

        public static string WordFinder(int a) // For taking words from dictionary as a input
        {
            return (dictionary[a]);
        }

        public static void QueryProcess() // Main query function
        {

            int fillAreaCounter = 0; // This counter will be length of word
            for (int i = 0; i < queryBag.Length; i++) // In this loop; we get informations about query for example how many letter does it contain which are unknown which are known
            {
                if (queryBag[i] != '_')
                {
                    fillAreaCounter++; // This counter gives us length of query words
                }
            }

            int queryListCounter = 0; // We will use this as an index for setting words in the queryresult list array
            for (int j = 0; j < dictionary.Length; j++) // We search dictionary in this loop
            {
                string word = WordFinder(j); // Calling function 
                if (word.Length == fillAreaCounter) // First condition is: Does the query word is as length as a word which came from dictionary
                {
                    char[] lettersOfWord = word.ToCharArray(); // If its same length we split it letter by letter to array
                    int pointCounter = 0; // This counter counts points which can be any letter in the reservoir
                    int commonLetterCounter = 0; // This counts given specific letters is takes place true position or not
                    for (int i = 0; i < fillAreaCounter; i++)
                    {
                        if (queryBag[i] == lettersOfWord[i])
                        {
                            commonLetterCounter++;
                        }
                        if (queryBag[i] == '.')
                        {
                            pointCounter++;
                        }

                    }
                    if (commonLetterCounter + pointCounter == fillAreaCounter) // Their sum must be equal to the length of word
                    {
                        if (queryListCounter <= 9) // We only show 10 result
                        {
                            queryResultList[queryListCounter] = word;
                            queryListCounter++;
                        }
                    }
                }
            }
        }

        public static void QueryScreen() // For using query function when you press tab you enter return screen if you press twice you face with query screen and use enter for query search
        {

            spx = 47;
            spy = 9;

            ConsoleKeyInfo cki;
            Console.SetCursorPosition(spx, spy);

            while (true)
            {
                Console.SetCursorPosition(spx, spy);
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.LeftArrow && spx > 47)
                {

                    spx -= 2;
                }
                else if (cki.Key == ConsoleKey.RightArrow && spx < 69)
                {
                    spx += 2;
                }
                if (((cki.KeyChar < 91 && cki.KeyChar > 64) || cki.KeyChar == 95 || cki.KeyChar == 46))
                {
                    Console.Write(cki.KeyChar.ToString());
                    queryBag[(spx - 47) / 2] = Convert.ToChar(cki.KeyChar);

                }

                if (cki.Key == ConsoleKey.Enter) // Enter for process
                {
                    QueryProcess();
                    char[] queryCleaner = new char[16] { '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_' };
                    for (int i = 0; i < queryBag.Length; i++)
                    {
                        queryBag[i] = queryCleaner[i];

                    }
                    break;
                }
            }

        }

        public static void ReturnProgress() // This is the main return function
        {
            if (round % 2 == 1) // Round determiner
            {

                for (int i = 0; i < returnbag.Length; i++) // For searching return bag one by one
                {
                    for (int j = 0; j < bag1.Length; j++) // For checking letters do they exist in the player bag
                    {

                        if (returnbag[i] == bag1[j])  // If they equal
                        {
                            for (int k = 0; k < 26; k++) // We find the wletter in the reservoir
                            {
                                if (bag1[j] == letters[k].let) // Is the letter equal
                                {
                                    letters[k].frequency++; // This process means we send back letter to reservoir because we increase it's frequency
                                    returnbag[i] = '_';
                                    bag1[j] = ' ';
                                    Refil(); // After necessary changes we call refil function to refil bags
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            else if (round % 2 == 0) // All process are same as we mentioned above the only difference is this is for the other player
            {
                for (int i = 0; i < returnbag.Length; i++)
                {

                    for (int j = 0; j < bag2.Length; j++)
                    {
                        if (returnbag[i] == bag2[j])
                        {
                            for (int k = 0; k < 26; k++)
                            {
                                if (bag2[j] == letters[k].let)
                                {
                                    letters[k].frequency++;
                                    returnbag[i] = '_';
                                    bag2[j] = ' ';
                                    Refil();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void ReturnScreen() // This screen for using return function
        {
            spx = 49;
            spy = 6;
            bool flag2 = true;
            ConsoleKeyInfo cki;
            Console.SetCursorPosition(spx, spy);

            while (true)
            {
                Console.SetCursorPosition(spx, spy);
                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.LeftArrow && spx > 49)
                {

                    spx -= 2;
                }
                else if (cki.Key == ConsoleKey.RightArrow && spx < 61)
                {
                    spx += 2;
                }
                if (((cki.KeyChar < 91 && cki.KeyChar > 64) || cki.KeyChar == 95))
                {
                    Console.Write(cki.KeyChar.ToString());
                    returnbag[(spx - 49) / 2] = Convert.ToChar(cki.KeyChar);

                }

                if (cki.Key == ConsoleKey.Backspace) // backspace calls the function for return process
                {
                    ReturnProgress();
                    char[] returnbarCleaner = new char[7] { '_', '_', '_', '_', '_', '_', '_' }; // We reset the return bag array for other rethurn process
                    for (int i = 0; i < returnbag.Length; i++)
                    {
                        returnbag[i] = returnbarCleaner[i];
                    }
                    break;
                }
                if (cki.Key == ConsoleKey.Tab) // Tab provide us query function
                {
                    QueryScreen();
                    querycontrol++;
                    if (querycontrol >= 4) // player can make query search only for 4 times maximum in a turn
                    {
                        checkround = false;
                        querycontrol = 0;

                    }
                    else
                    {
                        checkround = true;

                    }

                    break;
                }
            }

        }

        public static void Score()
        {
            int tempt = count - 1; // for writed last word
            int score = 0;
            int s = 0;

            while (s < counterOfWord) // if there are more than a meaningful word 
            {
                score = 0;

                for (int i = 0; i < letters.Length; i++)
                {
                    for (int j = ControlWords[tempt].word.Length - 1; j >= 0; j--)
                    {
                        if (letters[i].let == Convert.ToChar(ControlWords[tempt].word[j]))
                        {
                            score = score + letters[i].score;
                        }
                    }
                }

                if (player == 2)
                { score1 = score + score1; }
                else if (player == 1)
                { score2 = score + score2; }

                Console.Write(" + " + score); // display

                tempt--; // if there are more than a meaningful word in the last rount , tempt move back one-by-one and all the words that created in the last rount will be calculated
                s++;
                if (tempt <= 0) { break; }
            }
        }

        public static void Playing()
        {
            bool bagControl = false;
            returnCheck = false;

            ConsoleKeyInfo cki;

            Console.SetCursorPosition(spx, spy);
            bool flag = false;

            while (flag == false)
            {

                cki = Console.ReadKey(true);
                if (cki.Key == ConsoleKey.LeftArrow && spx > 2)
                {

                    spx -= 2;
                }
                else if (cki.Key == ConsoleKey.RightArrow && spx < 30)
                {
                    spx += 2;
                }
                else if (cki.Key == ConsoleKey.UpArrow && spy > 3)
                {

                    spy--;
                }
                else if (cki.Key == ConsoleKey.DownArrow && spy < 17)
                {
                    spy++;
                }
                Console.SetCursorPosition(spx, spy);

                if (((cki.KeyChar < 91 && cki.KeyChar > 64) || cki.KeyChar == 46) && control[spy - 3, (spx / 2) - 1] == '.') // if player has not letter that he/she wanted to lock , he/she can not.
                {
                    Console.Write(cki.KeyChar.ToString());
                    board[spy - 3, (spx / 2) - 1] = Convert.ToChar(cki.KeyChar);

                }

                if (cki.KeyChar == 13 && board[spy - 3, (spx / 2) - 1] != '.') // to lock the letter that player has 
                {
                    if (player == 1)
                    {
                        for (int i = 0; i < bag1.Length; i++)
                        {
                            if (bag1[i] == board[spy - 3, (spx / 2) - 1])
                            {
                                bagControl = true;
                                bag1[i] = ' ';
                                control[spy - 3, (spx / 2) - 1] = board[spy - 3, (spx / 2) - 1];
                                break;
                            }

                        }
                    }
                    else if (player == 2)
                    {
                        for (int i = 0; i < bag2.Length; i++)
                        {
                            if (bag2[i] == board[spy - 3, (spx / 2) - 1])
                            {
                                bagControl = true;
                                bag2[i] = ' ';
                                control[spy - 3, (spx / 2) - 1] = board[spy - 3, (spx / 2) - 1];
                                break;
                            }

                        }
                    }
                }

                if (cki.KeyChar == 32) // to lock the word
                {
                    isWord = false;

                    WordControl();

                    flag = true;
                    word = "";
                    finish++;
                    if (counterOfWord != 0)
                    {
                        finish = 0;
                    }

                    for (int i = 0; i < queryResultList.Length; i++)
                    {
                        queryResultList[i] = "";
                    }
                    if (finish >= 4)
                    {
                        isGameFinished = true;
                    }

                }

                if (cki.Key == ConsoleKey.Tab) // return
                {
                    ReturnScreen();
                    returnCheck = true;
                    spx = 2; spy = 3;
                    break;
                }
            }
        }

        public static void WordControl()
        {
            int counter = 0;
            counterOfWord = 0;  // word counter that crated meaningfull word in that round

            for (int i = 0; i < control.GetLength(0); i++) // horizontally control
            {
                for (int j = 0; j < control.GetLength(1); j++)
                {

                    if (control[i, j] != '.')
                    {
                        if (j - 1 == -1 || control[i, j - 1] == '.') // back-up coordinate for using same words situation
                        {
                            ControlWords[count].x = i;
                            ControlWords[count].y = j;
                        }

                        word = word.Insert(counter, Convert.ToString(control[i, j])); // adding letters

                        counter++; // counter to place letter end of the word that will be controlled
                    }

                    if (control[i, j] == '.' || j == control.GetLength(1) - 1)
                    {
                        counter = 0; // to return to the start of the new word that will be created for adding start of that
                        for (int k = 0; k < dictionary.Length; k++) // dictionary control
                        {

                            if (word == dictionary[k])
                            {

                                isWord = true;

                                for (int l = 0; l < ControlWords.Length; l++)
                                {

                                    if (ControlWords[l].word == word && count != l && ControlWords[count].x == ControlWords[l].x && ControlWords[count].y == ControlWords[l].y) // control for the same words situation
                                    {
                                        isWord = false;
                                    }
                                }

                                if (isWord == true)
                                {
                                    ControlWords[count].word = word; // word assign the array that contain used words
                                    counterOfWord++;
                                    count++;

                                    for (int a = 0; a < control.GetLength(0); a++)
                                    {
                                        for (int b = 0; b < control.GetLength(1); b++)
                                        {
                                            lastBoard[a, b] = control[a, b];
                                        }
                                    }
                                }
                            }
                        }

                        word = ""; // word reset
                    }
                }
            }

            counter = 0;
            word = "";
            // same with verically control
            for (int i = 0; i < control.GetLength(0); i++) // vertically control
            {
                for (int j = 0; j < control.GetLength(1); j++)
                {

                    if (control[j, i] != '.')
                    {
                        if (j - 1 == -1 || control[j - 1, i] == '.')
                        {
                            ControlWords[count].x = j;
                            ControlWords[count].y = i;
                        }

                        word = word.Insert(counter, Convert.ToString(control[j, i]));
                        counter++;
                    }

                    if (control[j, i] == '.' || j == control.GetLength(1) - 1)
                    {
                        counter = 0;
                        for (int k = 0; k < dictionary.Length; k++)
                        {

                            if (word == dictionary[k])
                            {
                                isWord = true;

                                for (int l = 0; l < ControlWords.Length; l++)
                                {

                                    if (ControlWords[l].word == word && count != l && ControlWords[count].x == ControlWords[l].x && ControlWords[count].y == ControlWords[l].y)
                                    {
                                        isWord = false;
                                    }

                                }

                                if (isWord == true)
                                {
                                    ControlWords[count].word = word;
                                    counterOfWord++;
                                    count++;

                                    for (int a = 0; a < control.GetLength(0); a++)
                                    {
                                        for (int b = 0; b < control.GetLength(1); b++)
                                        {
                                            lastBoard[a, b] = control[a, b];
                                        }
                                    }
                                }
                            }
                        }

                        word = "";
                    }
                }
            }


            if (counterOfWord == 0) // if player has not meaningful word, the letters that he/she used is gave back
            {
                if (player == 1)
                {
                    for (int i = 0; i < bag1.Length; i++)
                    {
                        bag1[i] = temptBag1[i];
                    }
                }
                else if (player == 2)
                {
                    for (int i = 0; i < bag2.Length; i++)
                    {
                        bag2[i] = temptBag2[i];
                    }
                }
                for (int a = 0; a < control.GetLength(0); a++)
                {
                    for (int b = 0; b < control.GetLength(1); b++)
                    {
                        control[a, b] = lastBoard[a, b];
                    }
                }

            }

        }

        public static void ReservoirControl()
        {
            int sumOfFrequency = 0;

            for (int i = 0; i < letters.Length; i++)
            {
                sumOfFrequency = letters[i].frequency + sumOfFrequency; // sumofthereservoir will be calculated every refill
            }

            if (sumOfFrequency == 0)
                isGameFinished = true;

        }

        public static void Refil()
        {
            for (int i = 0; i < bag1.Length; i++)
            {
                while (bag1[i] == ' ') // if this element has not letter, it will be filled
                {
                    ReservoirControl();
                    if (isGameFinished == true) { break; }

                    int number = rnd.Next(0, 26);
                    if (letters[number].frequency > 0)
                    {
                        temptBag1[i] = bag1[i] = letters[number].let;
                        letters[number].frequency--;
                    }
                }
            }

            for (int i = 0; i < bag2.Length; i++)
            {
                while (bag2[i] == ' ') // if this element has not letter, it will be filled
                {
                    ReservoirControl();
                    if (isGameFinished == true) { break; }

                    int number = rnd.Next(0, 26);
                    if (letters[number].frequency > 0)
                    {
                        temptBag2[i] = bag2[i] = letters[number].let;
                        letters[number].frequency--;
                    }
                }
            }

        }

        public static void Files()
        {
            StreamReader dictionaryText = File.OpenText("c:\\dictionary.txt");
            for (int i = 0; i < dictionary.Length; i++)
            {
                dictionary[i] = dictionaryText.ReadLine();
            }
            dictionaryText.Close();

            StreamReader LettersText = new StreamReader("c:\\letter_reservoir.txt");

            for (int i = 0; i < 26; i++)
            {
                string line = LettersText.ReadLine();
                string[] arr = line.Split(' ');

                letters[i].let = Convert.ToChar(arr[0]);
                letters[i].score = Convert.ToInt32(arr[1]);
                letters[i].frequency = Convert.ToInt32(arr[2]);
            }

            LettersText.Close();
        }

        public static void Board()
        {

            if (round % 2 == 1)
                player = 1;
            else
                player = 2;

            Console.SetWindowSize(Math.Min(150, Console.LargestWindowWidth), Math.Min(30, Console.LargestWindowHeight));
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;


            Console.WriteLine("Letter points:  A:1  B:3  C:3  D:2  E:1  F:4  G:2  H:4  I:1  J:8  K:5  L:1  M:3");
            Console.WriteLine("                N:1  O:1  P:3  Q:10 R:1  S:1  T:1  U:1  V:4  W:4  X:8  Y:4  Z:10");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("+--------------------------------+");
            for (int i = 0; i < lastBoard.GetLength(0); i++)
            {
                Console.Write("| ");
                for (int j = 0; j < lastBoard.GetLength(1); j++)
                {
                    Console.Write(lastBoard[i, j] + " ");
                }
                Console.Write(" |");
                Console.WriteLine();

            }
            Console.WriteLine("+--------------------------------+");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(40, 3);
            Console.Write("Round : " + round + " (Player " + player + ")");
            Console.SetCursorPosition(40, 6);
            Console.Write("Returns: ");
            for (int i = 0; i < returnbag.Length; i++)
            {
                Console.Write(returnbag[i] + " ");
            }
            Console.SetCursorPosition(40, 9);
            Console.Write("Query: ");

            for (int i = 0; i < queryBag.Length; i++)
            {
                Console.Write(queryBag[i] + " ");
            }

            for (int i = 0; i < queryResultList.Length; i++)
            {
                Console.SetCursorPosition(40, 10 + i);
                Console.Write(queryResultList[i]);
            }
            Console.SetCursorPosition(70, 3);
            Console.WriteLine("Player 1:");
            Console.SetCursorPosition(70, 6);
            Console.Write("Bag: ");
            for (int i = 0; i < bag1.Length; i++)
            {
                Console.Write(bag1[i] + " ");
            }
            Console.WriteLine();
            Console.SetCursorPosition(70, 9);
            Console.Write("Score: " + score1);
            if (round > 1 && player == 2 && counterOfWord > 0 && returnCheck == false) // displaying the score
            {
                Score();
                Console.Write(" = " + score1);
            }
            Console.SetCursorPosition(70, 12);
            Console.WriteLine("Player 2:");
            Console.SetCursorPosition(70, 15);
            Console.Write("Bag: ");
            for (int i = 0; i < bag2.Length; i++)
            {
                Console.Write(bag2[i] + " ");
            }
            Console.WriteLine();
            Console.SetCursorPosition(70, 18);
            Console.Write("Score: " + score2);
            if (round > 1 && player == 1 && counterOfWord > 0 && returnCheck == false) // displaying the score
            {
                Score();
                Console.Write(" = " + score2);
            }

        }

        public static void ArrayAssign(ref char[,] x)
        {
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    x[i, j] = '.';
                }
            }
        }

        static void Main(string[] args)
        {
            int x = 0;
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("                     ███████╗ ██████╗██████╗ ██╗██████╗ ██████╗ ██╗     ███████╗");
            Console.WriteLine("                     ██╔════╝██╔════╝██╔══██╗██║██╔══██╗██╔══██╗██║     ██╔════╝");
            Console.WriteLine("                     ███████╗██║     ██████╔╝██║██████╔╝██████╔╝██║     █████╗");
            Console.WriteLine("                     ╚════██║██║     ██╔══██╗██║██╔══██╗██╔══██╗██║     ██╔══╝");
            Console.WriteLine("                     ███████║╚██████╗██║  ██║██║██████╔╝██████╔╝███████╗███████╗");
            Console.WriteLine("                     ╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝╚═════╝ ╚═════╝ ╚══════╝╚══════╝");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("╔═╗╦═╗╔═╗╔═╗╔═╗                         ╔═╗╔═╗╔╗╔╦╗╦═╗╔═╗╦  ╔═╗");
            Console.WriteLine("╠═╝╠╦╝║╣ ╚═╗╚═╗    1           ───      ║  ║ ║║║║║ ╠╦╝║ ║║  ╚═╗");
            Console.WriteLine("╩  ╩╚═╚═╝╚═╝╚═╝                         ╚═╝╚═╝╝╚╝╩ ╩╚═╚═╝╩═╝╚═╝");
            Console.WriteLine("╔═╗╦═╗╔═╗╔═╗╔═╗                         ╔═╗╔═╗╔╦╗╔═╗╔═╗  ╦═╗╦ ╦╦  ╔═╗╔═╗");
            Console.WriteLine("╠═╝╠╦╝║╣ ╚═╗╚═╗    2            ───     ║ ╦╠═╣║║║║╣ ╚═╗  ╠╦╝║ ║║  ║╣ ╚═╗");
            Console.WriteLine("╩  ╩╚═╚═╝╚═╝╚═╝                         ╚═╝╩ ╩╩ ╩╚═╝╚═╝  ╩╚═╚═╝╩═╝╚═╝╚═╝");
            Console.WriteLine("╔═╗╦═╗╔═╗╔═╗╔═╗  ╔═╗╔╗╔╔╦╗╔═╗╦═╗        ╔═╗╔╦╗╔═╗╦═╗╔╦╗┬");
            Console.WriteLine("╠═╝╠╦╝║╣ ╚═╗╚═╗  ║╣ ║║║ ║ ║╣ ╠╦╝ ───    ╚═╗ ║ ╠═╣╠╦╝ ║ │");
            Console.WriteLine("╩  ╩╚═╚═╝╚═╝╚═╝  ╚═╝╝╚╝ ╩ ╚═╝╩╚═        ╚═╝ ╩ ╩ ╩╩╚═ ╩ o");
            ConsoleKeyInfo cki;
            bool choose = false;
            while (choose == false)
            {
                cki = Console.ReadKey(true);

                switch (cki.Key)
                {


                    case ConsoleKey.D1:
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("CONTROLS");
                            Console.WriteLine("-PRESS 'ENTER' AFTER EVERY LETTER THEN PRESS 'SPACE' AFTER YOU FİNİSH THE WORD");
                            Console.WriteLine(" YOU CAN'T LOCK THE LETTER IF YOU HAVE NOT THAT LETTER.)");
                            Console.WriteLine("-PRESS 'TAB' FOR RETURN AND WRITE THE LETTERS(YOU DON'T HAVE TO PRESS 'ENTER' AFTER EVERY LETTER.)");
                            Console.WriteLine(" IF YOU WANT TO FINISH THIS PROCESS, PRESS 'BACKSPACE'.");
                            Console.WriteLine("-PRESS TWO TIMES 'TAB' FOR QUERY AND WRİTE LETTERS AND DOTS (SAME WİTH RETURN).");
                            Console.WriteLine(" IF YOU WANT TO FINISH THIS PROCESS, PRESS 'ENTER'");

                            break;
                        }
                    case ConsoleKey.D2:
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("GAME'S RULES");
                            Console.WriteLine("-YOU CAN JUST USE YOUR LETTERS.");
                            Console.WriteLine("-YOU CAN USE THE QUERY THREE TIMES.");
                            Console.WriteLine("-YOU CAN WRITE SAME WORD MORE THAN ONE.");
                            Console.WriteLine("-IF YOU USE THE RETURN, YOUR TURN IS OVER.");
                            Console.WriteLine("-YOU CAN NOT RETURN LETTERS THAT YOU HAVE NOT.");
                            Console.WriteLine("-YOU CAN JUST USE THE QUERY THREE TIMES, THEN YOUR TURN IS FINISHED.");
                            Console.WriteLine("-IF LETTERS THAT ARE IN THE RESERVOIR IS FINISHED, GAME IS OVER.");
                            Console.WriteLine("-IF PLAYERS SAY CONSECUTIVELY PASS FOUR TIMES, GAME IS OVER.");
                            break;
                        }

                    case ConsoleKey.Enter:
                        choose = true;
                        break;
                }
            }

            Files();

            ArrayAssign(ref board);
            ArrayAssign(ref control);
            ArrayAssign(ref lastBoard);

            Refil(); //first filling of bags

            while (isGameFinished == false)
            {
                Board();
                Playing();
                Refil();

                if (checkround == false)
                {
                    round++;
                }
                else
                {
                    checkround = false;
                }

            }
            Console.Clear();
            if (score1 > score2)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("██╗    ██╗██╗███╗   ██╗███╗   ██╗███████╗██████╗     ██╗███████╗    ██████╗ ██╗      █████╗ ██╗   ██╗███████╗██████╗    ██╗    ");
                Console.WriteLine("██║    ██║██║████╗  ██║████╗  ██║██╔════╝██╔══██╗    ██║██╔════╝    ██╔══██╗██║     ██╔══██╗╚██╗ ██╔╝██╔════╝██╔══██╗  ███║    ");
                Console.WriteLine("██║ █╗ ██║██║██╔██╗ ██║██╔██╗ ██║█████╗  ██████╔╝    ██║███████╗    ██████╔╝██║     ███████║ ╚████╔╝ █████╗  ██████╔╝  ╚██║    ");
                Console.WriteLine("██║███╗██║██║██║╚██╗██║██║╚██╗██║██╔══╝  ██╔══██╗    ██║╚════██║    ██╔═══╝ ██║     ██╔══██║  ╚██╔╝  ██╔══╝  ██╔══██╗   ██║  ");
                Console.WriteLine("╚███╔███╔╝██║██║ ╚████║██║ ╚████║███████╗██║  ██║    ██║███████║    ██║     ███████╗██║  ██║   ██║   ███████╗██║  ██║   ██║    ");
                Console.WriteLine(" ╚══╝╚══╝ ╚═╝╚═╝  ╚═══╝╚═╝  ╚═══╝╚══════╝╚═╝  ╚═╝    ╚═╝╚══════╝    ╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝   ╚═╝ ");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔═╗╔═╗╔═╗╦═╗╔═╗    ");
                Console.WriteLine("╚═╗║  ║ ║╠╦╝║╣    == " + score1);
                Console.WriteLine("╚═╝╚═╝╚═╝╩╚═╚═╝    ");
                Console.WriteLine();

            }
            else if (score2 > score1)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("██╗    ██╗██╗███╗   ██╗███╗   ██╗███████╗██████╗     ██╗███████╗    ██████╗ ██╗      █████╗ ██╗   ██╗███████╗██████╗   ██████╗    ");
                Console.WriteLine("██║    ██║██║████╗  ██║████╗  ██║██╔════╝██╔══██╗    ██║██╔════╝    ██╔══██╗██║     ██╔══██╗╚██╗ ██╔╝██╔════╝██╔══██╗  ╚════██╗   ");
                Console.WriteLine("██║ █╗ ██║██║██╔██╗ ██║██╔██╗ ██║█████╗  ██████╔╝    ██║███████╗    ██████╔╝██║     ███████║ ╚████╔╝ █████╗  ██████╔╝   █████╔╝ ");
                Console.WriteLine("██║███╗██║██║██║╚██╗██║██║╚██╗██║██╔══╝  ██╔══██╗    ██║╚════██║    ██╔═══╝ ██║     ██╔══██║  ╚██╔╝  ██╔══╝  ██╔══██╗  ██╔═══╝  ");
                Console.WriteLine("╚███╔███╔╝██║██║ ╚████║██║ ╚████║███████╗██║  ██║    ██║███████║    ██║     ███████╗██║  ██║   ██║   ███████╗██║  ██║  ███████╗   ");
                Console.WriteLine(" ╚══╝╚══╝ ╚═╝╚═╝  ╚═══╝╚═╝  ╚═══╝╚══════╝╚═╝  ╚═╝    ╚═╝╚══════╝    ╚═╝     ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚══════╝╚═╝  ╚═╝  ╚══════╝ ");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("╔═╗╔═╗╔═╗╦═╗╔═╗    ");
                Console.WriteLine("╚═╗║  ║ ║╠╦╝║╣    == " + score2);
                Console.WriteLine("╚═╝╚═╝╚═╝╩╚═╚═╝    ");
                Console.WriteLine();

            }
            else if (score1 == score2)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("        ████████╗██╗███████╗██╗");
                Console.WriteLine("        ╚══██╔══╝██║██╔════╝██║");
                Console.WriteLine("           ██║   ██║█████╗  ██║");
                Console.WriteLine("           ██║   ██║██╔══╝  ╚═╝");
                Console.WriteLine("           ██║   ██║███████╗██╗");
                Console.WriteLine("           ╚═╝   ╚═╝╚══════╝╚═╝");
            }

            Console.ReadLine();

        }
    }
}
