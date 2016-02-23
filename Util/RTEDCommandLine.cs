//    Copyright (C) 2012  Mateusz Pawlik and Nikolaus Augsten
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Barbar.TreeDistance.Distance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Barbar.TreeDistance.Util
{
    /**
     * This is the command line access for running RTED algorithm.
     * 
     * @author Mateusz Pawlik
     *
     */
    public class RTEDCommandLine {

        private string helpMessage =
              "\n" +
              "Compute the edit distance between two trees.\n" +
              "\n" +
                  "SYNTAX\n" +
              "\n" +
              "  Simple Syntax -- use default algorithm (RTED):\n" +
              "\n" +
              "    RTED.exe {-t TREE1 TREE2 | -f FILE1 FILE2} [-c CD CI CR] [-v] [-m]\n" +
              "\n" +
              "    RTED.exe -h\n" +
              "\n" +
              "  Advanced Syntax -- use other algorithms or user-defined strategies:\n" +
              "\n" +
              "    RTED.exe {-t TREE1 TREE2 | -f FILE1 FILE2} -s {left|right|heavy}\n" +
              "        [-sw] [-c CD CI CR] [-v] [-m]\n" +
              "    RTED.exe {-t TREE1 TREE2 | -f FILE1 FILE2} -a FILE\n" +
              "        [-c CD CI CR] [-v] [-m]\n" +
              "    RTED.exe {-t TREE1 TREE2 | -f FILE1 FILE2} \n" +
              "        [-l | -r | -k | -d | -o] [-c CD CI CR] [-v] [-m]\n" +
              "\n" +
              "DESCRIPTION\n" +
              "\n" +
              "    Compute the edit distance between two trees. If not otherwise\n" +
              "    specified, the RTED algorithm by Pawlik and Augsten [1] is\n" +
              "    used. This algorithm uses the optimal path strategy.\n" +
              "\n" +
              "    Optionally, the tree edit distance can be computed using the\n" +
              "    strategies by Zhang and Shasha [2], Klein [3], Demaine et al. [4],\n" +
              "    or a combination of thereof. The trees are either specified on the\n" +
              "    command line (-t) or read from files (-f). The default output only\n" +
              "    prints the tree edit distance, the verbose output (-v) adds\n" +
              "    additional information such as runtime and strategy statistics.\n" +
              "\n" +
              "    In additon to the tree edit distance, the minimal edit mapping between\n" +
              "    two trees can be computed (-m). There might be multiple minimal edit\n" +
              "    mappings. This option computes only one of them.\n" +
              "\n" +
              "OPTIONS\n" +
              "\n" +
                  "    -h, --help \n" +
                  "        print this help message.\n" +
                  "\n" +
                  "    -t TREE1 TREE2,\n" +
                  "    --trees TREE1 TREE2\n" +
                  "        compute the tree edit distance between TREE1 and TREE2. The\n" +
                  "        trees are encoded in the bracket notation, for example, in tree\n" +
                  "        {A{B{X}{Y}{F}}{C}} the root node has label A and two children\n" +
                  "        with labels B and C. B has three children with labels X, Y, F.\n" +
                  "\n" +
                  "    -f FILE1 FILE2, \n" +
                  "    --files FILE1 FILE2\n" +
                  "        compute the tree edit distance between the two trees stored in\n" +
                  "        the files FILE1 and FILE2. The trees are encoded in bracket\n" +
                  "        notation.\n" +
                  "\n" +
                  "    -c CD CI CR, \n" +
                  "    --costs CD CI CR\n" +
                  "        set custom cost for edit operations. Default is -c 1 1 1.\n" +
                  "        CD - cost of node deletion\n" +
                  "        CI - cost of node insertion\n" +
                  "        CR - cost of node renaming\n" +
                  "\n" +
                  "    -s {left|right|heavy}, \n" +
                  "    --strategy {left|right|heavy}\n" +
                  "        set custom strategy that uses exclusively left, right, or\n" +
                  "        heavy paths.\n" +
                  "\n" +
                  "    -w, --switch\n" +
                  "        force to switch trees if the left-hand tree is smaller than\n" +
                  "        the right-hand tree.\n" +
                  "\n" +
                  "    -a FILE\n" +
                  "    --strategy-array FILE\n" +
                  "        read the strategy from FILE. Rows in the file represent\n" +
                "        subtrees of the left-hand tree in postorder, columns represent\n" +
            "        subtrees of the right-hand tree in postorder. Strategies are\n" +
            "        separated with space.  Use digits 0, 1, 2 for left, right, and\n" +
            "        heavy path in the left-hand tree and 4, 5, 6 for left, right,\n" +
            "        and heavy path in the right-hand tree. Example strategy for\n" +
            "        two trees with three nodes each:\n" +
                  "            0 1 2\n" +
                  "            4 5 6\n" +
                  "            2 6 0\n" +
                  "\n" +
                  "    -v, --verbose\n" +
                  "        print verbose output, including tree edit distance, runtime,\n" +
                  "        number of relevant subproblems and strategy statistics.\n" +
                  "\n" +
                  "    -l, --ZhangShashaLeft\n" +
                  "        like \"-s left\". Use the algorithm by Zhang and Shasha [2] with\n" +
                  "        left paths.\n" +
                  "\n" +
                  "    -r, --ZhangShashaRight\n" +
                  "        like \"-s right\". Use the algorithm by Zhang and Shasha [2] with\n" +
                  "        right paths.\n" +
                  "\n" +
                  "    -k, --Klein\n" +
                  "        like \"-s heavy\". Use the algorithm by Klein [3], which uses\n" +
                  "        heavy paths.\n" +
                  "\n" +
                  "    -d, --Demaine\n" +
                  "        like \"-s heavy -w\". Use the algorithm by Demaine [4], which\n" +
                  "        uses heavy paths and always decomposes the larger tree.\n" +
                  "\n" +
                  "    -o, --RTED\n" +
                  "        use the RTED algorithm by Pawlik and Augsten [1]. This is the\n" +
                  "        default strategy.\n" +
                  "\n" +
                  "    -m, --mapping\n" +
                  "        compute the minimal edit mapping between two trees. There might\n" +
                  "        be multiple minimal edit mappings. This option computes only one\n" +
                  "        of them. The frst line of the output is the cost of the mapping.\n" +
                  "        The following lines represent the edit operations. n and m are\n" +
                  "        postorder IDs (beginning with 1) of nodes in the left-hand and\n" +
                  "        the rigt-hand trees respectively.\n" +
                  "            n->m - rename node n to m\n" +
                  "            n->0 - delete node n\n" +
                  "            0->m - insert node m\n" +
                  "\n" +
              "EXAMPLES\n" +
              "\n" +
              "    RTED.exe -t {a{b}{c}} {a{b{d}}} -c 1 1 0.5 -s heavy --switch\n" +
              "    RTED.exe -f 1.tree 2.tree -s left\n" +
              "    RTED.exe -t {a{b}{c}} {a{b{d}}} --ZhangShashaLeft -v\n" +
              "\n" +
              "REFERENCES\n" +
              "\n" +
              "    [1] M. Pawlik and N. Augsten. RTED: a robust algorithm for the\n" +
              "        tree edit distance. Proceedings of the VLDB Endowment\n" +
              "        (PVLDB). 2012 (To appear).\n" +
              "    [2] K.Zhang and D.Shasha. Simple fast algorithms for the editing\n" +
              "        distance between trees and related problems. SIAM\n" +
              "        J. Computing. 1989.\n" +
              "    [3] P.N. Klein. Computing the edit-distance between unrooted\n" +
              "        ordered trees.  European Symposium on Algorithms (ESA). 1998.\n" +
              "    [4] E.D. Demaine, S. Mozes, B. Rossman and O. Weimann. An optimal\n" +
              "        decomposition algorithm for tree edit distance. ACM Trans. on\n" +
              "        Algorithms. 2009.\n" +
              "\n" +
              "AUTHORS\n" +
              "\n" +
              "    Mateusz Pawlik, Nikolaus Augsten";

        private string wrongArgumentsMessage = "Wrong arguments. Try \"RTED.exe --help\" for help.";

        private LblTree lt1, lt2;
        private int size1, size2;
        private bool run, custom, array, strategy, ifSwitch, sota, verbose, demaine, mapping;
        private int sotaStrategy;
        private string customStrategy, customStrategyArrayFile;
        private RTED_InfoTree_Opt rted;
        private double ted;

        /**
         * Main method 
         * 
         * @param args
         */
        public static void Main(string[] args) {
            RTEDCommandLine rtedCL = new RTEDCommandLine();
            rtedCL.runCommandLine(args);
        }

        /**
         * Run the command line with given arguments.
         * 
         * @param args
         */
        public void runCommandLine(string[] args) {
            rted = new RTED_InfoTree_Opt(1, 1, 1);

            try {
                for (int i = 0; i < args.Length; i++) {
                    if (args[i].Equals("--help") || args[i].Equals("-h")) {
                        Console.Out.WriteLine(helpMessage);
                        Environment.Exit(0);
                    } else if (args[i].Equals("-t") || args[i].Equals("--trees")) {
                        parseTreesFromCommandLine(args[i + 1], args[i + 2]);
                        i = i + 2;
                        run = true;
                    } else if (args[i].Equals("-f") || args[i].Equals("--files")) {
                        parseTreesFromFiles(args[i + 1], args[i + 2]);
                        i = i + 2;
                        run = true;
                    } else if (args[i].Equals("-l") || args[i].Equals("--ZhangShashaLeft")) {
                        sota = true;
                        sotaStrategy = 0;
                        strategy = true;
                    } else if (args[i].Equals("-r") || args[i].Equals("--ZhangShashaRight")) {
                        sota = true;
                        sotaStrategy = 1;
                        strategy = true;
                    } else if (args[i].Equals("-k") || args[i].Equals("--Klein")) {
                        sota = true;
                        sotaStrategy = 2;
                        strategy = true;
                    } else if (args[i].Equals("-d") || args[i].Equals("--Demaine")) {
                        sota = true;
                        demaine = true;
                        sotaStrategy = 2;
                        strategy = true;
                    } else if (args[i].Equals("-o") || args[i].Equals("--RTED")) {
                        // do nothing - this is the default option
                    } else if (args[i].Equals("-s") || args[i].Equals("--strategy")) {
                        custom = true;
                        customStrategy = args[i + 1];
                        i = i + 1;
                        strategy = true;
                    } else if (args[i].Equals("-a") || args[i].Equals("--strategy-array")) {
                        array = true;
                        customStrategyArrayFile = args[i + 1];
                        i = i + 1;
                        strategy = true;
                    } else if (args[i].Equals("-w") || args[i].Equals("--switch")) {
                        ifSwitch = true;
                    } else if (args[i].Equals("-c") || args[i].Equals("--costs")) {
                        setCosts(args[i + 1], args[i + 2], args[i + 3]);
                        i = i + 3;
                    } else if (args[i].Equals("-v") || args[i].Equals("--verbose")) {
                        verbose = true;
                    } else if (args[i].Equals("-m") || args[i].Equals("--mapping")) {
                        mapping = true;
                    } else {
                        Console.Out.WriteLine(wrongArgumentsMessage);
                        Environment.Exit(0);
                    }
                }

            } catch (IndexOutOfRangeException e) {
                Console.Out.WriteLine("Too few arguments.");
                Environment.Exit(0);
            }

            if (!run) {
                Console.Out.WriteLine(wrongArgumentsMessage);
                Environment.Exit(0);
            }

            var watch = new Stopwatch();
            watch.Start();
            if (strategy) {
                if (sota) {
                    if (demaine) {
                        setStrategy(sotaStrategy, true);
                    } else {
                        setStrategy(sotaStrategy, false);
                    }
                    ted = rted.nonNormalizedTreeDist();
                } else if (custom) {
                    setStrategy(customStrategy, ifSwitch);
                    ted = rted.nonNormalizedTreeDist();
                } else if (array) {
                    setStrategy(customStrategyArrayFile);
                    ted = rted.nonNormalizedTreeDist();
                }
            } else {
                rted.computeOptimalStrategy();
                ted = rted.nonNormalizedTreeDist();
            }
            watch.Stop();
            if (verbose) {
                Console.Out.WriteLine("distance:             " + ted);
                Console.Out.WriteLine("runtime:              " + watch.ElapsedMilliseconds / 1000.0);
                Console.Out.WriteLine("relevant subproblems: " + rted.counter);
                Console.Out.WriteLine("recurence steps:      " + rted.strStat[3]);
                Console.Out.WriteLine("left paths:           " + rted.strStat[0]);
                Console.Out.WriteLine("right paths:          " + rted.strStat[1]);
                Console.Out.WriteLine("heavy paths:          " + rted.strStat[2]);
            } else {
                Console.Out.WriteLine(ted);
            }
            if (mapping) {
                var editMapping = rted.computeEditMapping();
                foreach (int[] nodeAlignment in editMapping) {
                    Console.Out.WriteLine(nodeAlignment[0] + "->" + nodeAlignment[1]);
                }
            }
        }

        /**
         * Parse two input trees from the command line.
         * 
         * @param ts1
         * @param ts2
         */
        private void parseTreesFromCommandLine(string ts1, string ts2) {
            try {
                lt1 = LblTree.fromString(ts1);
                size1 = lt1.getNodeCount();
            } catch (Exception e) {
                Console.Out.WriteLine("TREE1 argument has wrong format");
                Environment.Exit(0);
            }
            try {
                lt2 = LblTree.fromString(ts2);
                size2 = lt2.getNodeCount();
            } catch (Exception e) {
                Console.Out.WriteLine("TREE2 argument has wrong format");
                Environment.Exit(0);
            }
            rted.init(lt1, lt2);
        }

        /**
         * Parse two input trees from given files.
         * 
         * @param fs1
         * @param fs2
         */
        private void parseTreesFromFiles(string fs1, string fs2) {
            try {
                lt1 = LblTree.fromString(new StreamReader(File.OpenRead(fs1)).ReadLine());
                size1 = lt1.getNodeCount();
            } catch (Exception e) {
                Console.Out.WriteLine("TREE1 argument has wrong format");
                Environment.Exit(0);
            }
            try {
                lt2 = LblTree.fromString(new StreamReader(File.OpenRead(fs2)).ReadLine());
                size2 = lt2.getNodeCount();
            } catch (Exception e) {
                Console.Out.WriteLine("TREE2 argument has wrong format");
                Environment.Exit(0);
            }
            rted.init(lt1, lt2);
        }

        /**
         * Set custom costs.
         * 
         * @param cds
         * @param cis
         * @param cms
         */
        private void setCosts(string cds, string cis, string cms) {
            try {
                rted.setCustomCosts(double.Parse(cds), double.Parse(cis), double.Parse(cms));
            } catch (Exception e) {
                Console.Out.WriteLine("One of the costs has wrong format.");
                Environment.Exit(0);
            }
        }

        /**
         * Set the strategy to be entirely of the type given by str.
         * 
         * @param str strategy type
         * @param ifSwitch if set to true the strategy will be applied to the currently bigger tree 
         */
        private void setStrategy(string str, bool ifSwitch) {
            if (str.Equals("left")) {
                rted.setCustomStrategy(0, ifSwitch);
            } else if (str.Equals("right")) {
                rted.setCustomStrategy(1, ifSwitch);
            } else if (str.Equals("heavy")) {
                rted.setCustomStrategy(2, ifSwitch);
            } else {
                Console.Out.WriteLine("Wrong strategy.");
                Environment.Exit(0);
            }
        }

        /**
         * Set the strategy to be entirely of the type given by str.
         * 
         * @param str strategy type
         * @param ifSwitch if set to true the strategy will be applied to the currently bigger tree 
         */
        private void setStrategy(int str, bool ifSwitch) {
            try {
                rted.setCustomStrategy(str, ifSwitch);
            } catch (Exception e) {
                Console.Out.WriteLine("Strategy has wrong format.");
                Environment.Exit(0);
            }
        }

        /**
         * Set the strategy to the one given in strArrayFile.
         * 
         * @param strArrayFile path to the file with the strategy
         */
        private void setStrategy(string strArrayFile) {
            try {
                rted.setCustomStrategy(parseStrategyArrayString(strArrayFile));
            } catch (Exception e) {
                Console.Out.WriteLine("Strategy has wrong format.");
                Environment.Exit(0);
            }
        }

        /**
         * Parse the strategy array.
         * 
         * Array string format:
         * ? ? ? ?
         * ? ? ? ? 
         * ? ? ? ? 
         * 
         * @param strategyArray
         * @return
         */
        private int[][] parseStrategyArrayString(string fileWithStrategyArray) {
            int[][] str = null;
            var strVector = new List<int[]>();
            int[] strLine;
            string line;
            Scanner s;
            int value;
            StreamReader br;

            try {
                br = new StreamReader(File.OpenRead(fileWithStrategyArray));
                line = br.ReadLine();
                int index = 0;
                while (line != null) {
                    s = new Scanner(line);
                    strLine = new int[(line.Length + 1) / 2];
                    if (strLine.Length != size2) {
                        Console.Error.WriteLine("Trees sizes differ from the strategy array dimensions.");
                        Environment.Exit(0);
                    }
                    int i = 0;
                    while (s.hasNextInt()) {
                        value = s.nextInt();
                        if (value != 0 && value != 1 && value != 2 && value != 4 && value != 5 && value != 6) {
                            Console.Out.WriteLine("Strategy value at position " + index + " in the strategy array file is wrong.");
                            Environment.Exit(0);
                        }
                        index++;
                        strLine[i] = value;
                        i++;
                    }
                    strVector.Add(strLine);
                    line = br.ReadLine();
                }
                str = new int[strVector.Count][];
                int j = 0;
                foreach (int[] l in strVector) {
                    str[j] = l;
                    j++;
                }
                if (str.Length != size1) {
                    Console.Error.WriteLine("Trees sizes differ from the strategy array dimensions.");
                    Environment.Exit(0);
                }
                br.Close();
            } catch (Exception e) {
                Console.Out.WriteLine("Something is wrong with strategy array file.");
                Environment.Exit(0);
            }

            return str;
        }
    }
}