using System;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Alice_s_guide_to_creativity
{

    class Program
    {
        #region stuff for fulscreen
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int HIDE = 0;
        private const int MAXIMIZE = 3;
        private const int MINIMIZE = 6;
        private const int RESTORE = 9;
        #endregion
        public static int height = 10;
        public static int width = 230;
        public static int[] startText = new int[2];
        public static (int y, int x) offset;
        public static int textHeight = 1;
        public static int textLength;
        public static int outputLines;
        public static int currentIndex = 0;
        public static int[] feelingsInput = new int[3];
        public static int[] index = new int[6];
        public static int laugh = 0;
        public static int cry = 0;
        public static int sad = 0;
        public static int laughThreshold = 6;
        public static int cryThreshold = 7;
        public static int sadThreshold = 4;
        public static int dialogueVariables = 4;

        public static string full = "█";
        public static string top = "▀";
        public static string bottom = "▄";
        public static string space = " ";
        public static string lineBreak = "~";
        public static string input;
        public static string line;
        public static string[] textDB;
        public static string[] happyDialogue;
        public static string[] laughingDialogue;
        public static string[] sadDialogue;
        public static string[] cryingDialogue;
        public static string[] pentamentoDialogue;

        public static (CanvasImage Emotion, string Response,(int laugh, int sad, int cry) feelings, string Color)[] dialogue;
        
        public static string[,] outputText;
        public static string[] words = new string[100];
        public static string[,] chatbox;

        public static bool pentamentoActive = false;
        public static bool QRCodeDrawn = false;

        //put in absolute path here to these images
        public static CanvasImage AliceHappy    =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceHappy.png");
        public static CanvasImage AliceSad      =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AlicePout.png");
        public static CanvasImage AliceXD       =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceXD.png");
        public static CanvasImage AliceCry      =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceCry.png");
        public static CanvasImage AliceUwU      =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceUwU.png");
        public static CanvasImage AliceOut      =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceOut.png");
        public static CanvasImage QRcode        =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\QRCode.png");

        static void Main(string[] args)
        {
            // Initiate some variables
            outputLines = height - 2;
            chatbox = new string[height,width];
            offset.y = textHeight;
            offset.x = 6;
            textLength = width - 50;
            outputText = new string[outputLines, textLength];

            // maximize console screen
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);  
            ShowWindow(ThisConsole, MAXIMIZE);
            
            AliceHappy.MaxWidth(48);

            //start program
            LoadDialogue();
            GenerateChatBox();
            while (true)
            {
                //update program
                ClearText();
                FrameUpdate(dialogue[currentIndex].Emotion, dialogue[currentIndex].Response);
                InputHandler();
            }
        }
        public static void LoadDialogue()
        {
            //go through text file, for every new line => add to dialogue.Emotion and dialogue.Response

            //put in absolute path to these text files
            happyDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\HappyDialogue.txt");
            sadDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\SadDialogue.txt");
            cryingDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\CryingDialogue.txt");
            laughingDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\LaughingDialogue.txt");
            pentamentoDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\PentamentoDialogue.txt");
            //fill index 0 with a 1 and index 1 to index Length with a 0
            index[0] = 1;
            for (int i = 1; i < index.Length; i++)
            {
                index[i] = 0;
            }
            //set the active text file equal to happyDialogue
            textDB = happyDialogue;
            UpdateDialogue();
        }
        public static void UpdateDialogue()
        {
            // initiate Dialogue which is an array of tuples
            dialogue = new (CanvasImage Emotion, string Response, (int laugh, int sad, int cry) feelings, string Color)[textDB.Length / dialogueVariables];

            //split based on line
            for (int i = 0; i < textDB.Length - dialogueVariables + 1; i++)
            {
                //Line 1 is Emotion
                if (i % dialogueVariables == 0)
                {
                    switch (textDB[i])
                    {
                        case "Cry":
                            dialogue[i / dialogueVariables].Emotion = AliceCry;
                            break;
                        case "Sad":
                            dialogue[i / dialogueVariables].Emotion = AliceSad;
                            break;
                        case "Laugh":
                            dialogue[i / dialogueVariables].Emotion = AliceXD;
                            break;
                        case "UwU":
                            dialogue[i / dialogueVariables].Emotion = AliceUwU;
                            break;
                        case "Out":
                            dialogue[i / dialogueVariables].Emotion = AliceOut;
                            break;
                        default:
                            dialogue[i / dialogueVariables].Emotion = AliceHappy;
                            break;
                    }
                }
                //Line 2 is Text
                else if (i % dialogueVariables == 1)
                {
                    dialogue[i / dialogueVariables].Response = textDB[i];
                }
                //Line 3 is the change in emotion variables on a positive or negative answer
                else if (i % dialogueVariables == 2)
                {
                    // turn string into string array into int array of feelings
                    feelingsInput = Array.ConvertAll(textDB[i].Split(','), int.Parse);
                    dialogue[i / dialogueVariables].feelings.laugh = feelingsInput[0];
                    dialogue[i / dialogueVariables].feelings.sad = feelingsInput[1];
                    dialogue[i / dialogueVariables].feelings.cry = feelingsInput[2];
                }
                //Line 4 is the color of the text and chatbox
                else
                {
                    dialogue[i / dialogueVariables].Color = textDB[i];
                }
            }
        }
        public static void GenerateChatBox()
        {
            //filling the chatbox array with the border of the chatbox
            //shush I hate switch cases
            for(int i = 0; i <= height-1 ; i++)
            {
                for(int j = 0; j <= width-1 ; j++)
                {
                    if (i == 0)
                    {
                        if(j == 1 || j == width-2)
                        {
                            chatbox[i, j] = bottom;
                        }
                        else
                        {
                            chatbox[i, j] = top;
                        }
                    }
                    else if(j == 0 && i != height-1)
                    {
                        chatbox[i, j] = full;
                    }
                    else if(j == width-1 && i != height-1)
                    {
                        chatbox[i, j] = full;
                    }
                    else if (i == height-1)
                    {
                        if (j == 1 || j == width - 2)
                        {
                            chatbox[i, j] = top;
                        }
                        else
                        {
                            chatbox[i, j] = bottom;
                        }
                    }
                    else
                    {
                        chatbox[i, j] = space;
                    }
                }
            }
        }
        public static void TextToArray(string target)
        {
            int counter = 0;
            int currentPosition = 0;
            //clear the words array and outputText array
            Array.Clear(words, 0, words.Length);
            Array.Clear(outputText, 0, outputText.Length);
            //split the input string into seperate words by splitting the array on every space
            words = target.Split(' ');

            //loop through the different lines available
            for (int i = 0; i < height -2; i++)
            {
                //set current position on 0 every new line
                currentPosition = 0;
                //loop through all words while the next word still fits on the line
                while(currentPosition < textLength && counter < words.Length)
                {
                    // if the word fits the current line, check if the next word is a linebreak, if not then add the characters to outputText and then add a space behind it.
                    if (currentPosition + words[counter].Length < textLength)
                    {
                        if (words[counter] == lineBreak)
                        {
                            counter++;
                            break;
                        }
                        else
                        {
                            foreach (char character in words[counter])
                            {
                                outputText[i, currentPosition] = character.ToString();
                                currentPosition++;
                            }
                            outputText[i, currentPosition] = " ";
                            currentPosition++;
                            counter++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }   
            }
            UpdateText();
        }
        public static void ClearText()
        {
            //loop through the chatbox array and delete anything that isn't the border
            for (int i = 0; i <= height - 1; i++)
            {
                for (int j = 0; j <= width - 1; j++)
                {
                    if(chatbox[i,j] != bottom && chatbox[i, j] != full && chatbox[i, j] != top)
                    {
                        chatbox[i, j] = space;
                    }
                }
            }
        }
        public static void UpdateText()
        {
            //loop through the chatbox array and fill in the characters in their respective index
            for (int i = 0; i < outputLines; i++)
            {
                for (int j = 0; j < textLength; j++)
                {
                    if(outputText[i,j] != null)
                    {
                        chatbox[i + offset.y, j + offset.x] = outputText[i, j];
                    } 
                }
            }
        }
        public static void FrameUpdate(CanvasImage image, string printText)
        {
            //clear the frame
            Console.Clear();
            //load in the Emotion image
            AnsiConsole.Render(image);
            //turn the input text into an array to process it into the chatbox array
            TextToArray(printText);
            //loop through every line of the chatbox
            for (int i = 0; i < height; i++)
            {
                //clear the line every time it goes to the next line in the chatbox
                line = "";
                //loop through the current line and fill the line string with that current line's values
                for(int j = 0; j < width; j++)
                {
                    line += chatbox[i, j]; 
                }
                //draw the line string according to the input color
                AnsiConsole.MarkupLine($"[{dialogue[currentIndex].Color}]" + line + "[/]");
            }
            // on the first frame of pentamento, draw a QR Code
            if (pentamentoActive && !QRCodeDrawn)
            {
                Console.WriteLine("");
                AnsiConsole.Render(QRcode);
                QRCodeDrawn = true;
            }
        }
        public static void InputHandler()
        {
            //read the user input
            input = Console.ReadLine().ToLower();
            //check what the user input, 1 will add the change in emotions of this text, 2 will subtract it, 3 won't do anything with it,
            ////pentamento will swap to my vijfluik project and anything else won't do anything as if a neutral answer
            switch (input)
            {
                case "1":
                    sad += dialogue[currentIndex].feelings.sad;
                    laugh += dialogue[currentIndex].feelings.laugh;
                    cry += dialogue[currentIndex].feelings.cry;
                    break;
                case "2":
                    sad -= dialogue[currentIndex].feelings.sad;
                    laugh -= dialogue[currentIndex].feelings.laugh;
                    cry -= dialogue[currentIndex].feelings.cry;
                    break;
                case "3":
                    break;
                case "pentamento":
                    pentamentoActive = true;
                    break;
                case null:
                    break;
                default:
                    break;
            }
            //go to the next index of the text file
            currentIndex++;
            TextHandler();
        }
        public static void TextHandler()
        {
            index[index[0]] = currentIndex;
            //if emotions changed enough to swap text files, save currentIndex in previous index -> set currentIndex equal to new index
            if (laugh >= laughThreshold)
            {
                
                index[0] = 2;
                textDB = laughingDialogue;
                currentIndex = index[2];
            }
            else if (sad >= sadThreshold)
            {
                index[0] = 3;
                textDB = sadDialogue;
                currentIndex = index[3];
            }
            else if (cry >= cryThreshold)
            {
                index[0] = 4;
                textDB = cryingDialogue;
                currentIndex = index[4];
            }
            //swap to pentamento dialogue 
            else if (pentamentoActive)
            {
                index[0] = 5;
                textDB = pentamentoDialogue;
                currentIndex = index[5];
            }
            else
            {
                index[0] = 1;
                textDB = happyDialogue;
                currentIndex = index[1];
            }
            UpdateDialogue();
        }
    }
}