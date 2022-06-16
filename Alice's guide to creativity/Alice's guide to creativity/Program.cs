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
        // laugh > 7 = laugh file
        // sad < -6 && laugh < 7 = sad file (here cry gets added)
        // cry > 8 = cry file
        // anything else = happy file

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

        public static CanvasImage AliceHappy    =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceHappy.png");
        public static CanvasImage AliceSad      =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AlicePout.png");
        public static CanvasImage AliceXD       =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceXD.png");
        public static CanvasImage AliceCry      =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceCry.png");
        public static CanvasImage AliceUwU      =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceUwU.png");
        public static CanvasImage AliceOut      =  new CanvasImage(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\AliceOut.png");

        static void Main(string[] args)
        {
            
            outputLines = height - 2;
            chatbox = new string[height,width];
            offset.y = textHeight;
            offset.x = 6;
            textLength = width - 50;
            outputText = new string[outputLines, textLength];


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
            //

            //go through text file, for every new line => add to dialogue.Emotion and dialogue.Response
            happyDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\HappyDialogue.txt");
            sadDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\SadDialogue.txt");
            cryingDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\CryingDialogue.txt");
            laughingDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\LaughingDialogue.txt");
            pentamentoDialogue = System.IO.File.ReadAllLines(@"C:\Users\Joey\Documents\GitHubSL\Alices-guide-to-projects\PentamentoDialogue.txt");
            index[0] = 1;
            for (int i = 1; i < index.Length; i++)
            {
                index[i] = 0;
            }
            textDB = happyDialogue;
            UpdateDialogue();
        }
        public static void UpdateDialogue()
        {
            dialogue = new (CanvasImage Emotion, string Response, (int laugh, int sad, int cry) feelings, string Color)[textDB.Length / dialogueVariables];
            //split based on line
            for (int i = 0; i < textDB.Length - dialogueVariables + 1; i++)
            {
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
                else if (i % dialogueVariables == 1)
                {
                    dialogue[i / dialogueVariables].Response = textDB[i];
                }
                else if (i % dialogueVariables == 2)
                {
                    // turn string into string array into int array of feelings
                    feelingsInput = Array.ConvertAll(textDB[i].Split(','), int.Parse);
                    dialogue[i / dialogueVariables].feelings.laugh = feelingsInput[0];
                    dialogue[i / dialogueVariables].feelings.sad = feelingsInput[1];
                    dialogue[i / dialogueVariables].feelings.cry = feelingsInput[2];
                }
                else
                {
                    dialogue[i / dialogueVariables].Color = textDB[i];
                }

            }
        }
        public static void GenerateChatBox()
        {
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

            Array.Clear(words, 0, words.Length);
            Array.Clear(outputText, 0, outputText.Length);
                words = target.Split(' ');

            
            for (int i = 0; i < height -2; i++)
            {
                currentPosition = 0;
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
            #region debug text
            //foreach (string word in words)
            //{
            //    Console.Write(word + " ");
            //}
            //Console.WriteLine("");
            //foreach (string character in outputText)
            //{
            //    Console.Write(character);
            //}
            #endregion
            
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
            Console.Clear();
            AnsiConsole.Render(image);
            TextToArray(printText);
            for (int i = 0; i < height; i++)
            {
                line = "";
                for(int j = 0; j < width; j++)
                {
                    line += chatbox[i, j]; 
                }
                AnsiConsole.MarkupLine($"[{dialogue[currentIndex].Color}]" + line + "[/]");
            }
            
        }

        public static void InputHandler()
        {
            input = Console.ReadLine().ToLower();
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
            currentIndex++;
            TextHandler();
        }
        public static void TextHandler()
        {
            index[index[0]] = currentIndex;
            if (laugh >= laughThreshold)
            {
                //save currentIndex in previous index -> set currentIndex equal to new index
                //index[0] = previous index (can be 1,2 or 3
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
// █ , ▄ , ▀


/*
 *
 * ▀▄▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▄▀
 * █    Hi I'm Alice, I'm here to ask some questions, are you ok with that?                                                   █
 * █    (type 'Y' for yes or 'N' for no)                                                                                      █
 * █                                                                                                                          █
 * █                                                                                                                          █
 * ▄▀▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▀▄
*/
