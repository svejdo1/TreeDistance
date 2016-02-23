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

using Barbar.TreeDistance.Util;
using System.Collections;
using System.Collections.Generic;

namespace Barbar.TreeDistance.Distance
{

    /**
     * Stores all needed information about a single tree in several indeces. 
     *
     * @author Mateusz Pawlik
     */
    public class InfoTree {

        private LblTree inputTree;

        private const byte LEFT = 0;
        private const byte RIGHT = 1;
        private const byte HEAVY = 2;
        private const byte BOTH = 3;
        private const byte REVLEFT = 4;
        private const byte REVRIGHT = 5;
        private const byte REVHEAVY = 6;

        // constants for indeces numbers
        public const byte POST2_SIZE = 0;
        public const byte POST2_KR_SUM = 1;
        public const byte POST2_REV_KR_SUM = 2;
        public const byte POST2_DESC_SUM = 3; // number of subforests in full decomposition
        public const byte POST2_PRE = 4;
        public const byte POST2_PARENT = 5;
        public const byte POST2_LABEL = 6;
        public const byte KR = 7; // key root nodes (size of this array = leaf count)
        public const byte POST2_LLD = 8; // left-most leaf descendants
        public const byte POST2_MIN_KR = 9; // minimum key root nodes index in KR array
        public const byte RKR = 10; // reversed key root nodes
        public const byte RPOST2_RLD = 11; // reversed postorer 2 right-most leaf descendants
        public const byte RPOST2_MIN_RKR = 12;
        public const byte RPOST2_POST = 13; // reversed postorder -> postorder
        public const byte POST2_STRATEGY = 14; // strategy for Demaine (is there sth on the left/right of the heavy node)
        public const byte PRE2_POST = 15;

        public int[][] info; // an array with all the indeces

        private LabelDictionary ld; // dictionary with labels - common for two input trees

        public bool[][] nodeType; // store the type of a node: for every node stores three bool values (L, R, H)

        // paths and rel subtrees are inside 2D arrays to be able to get them by paths/relsubtrees[L/R/H][node]
        private int[][] paths;
        private int[][][] relSubtrees;

        // temporal variables
        private int sizeTmp = 0; // temporal value of size of a subtree
        private int descSizesTmp = 0; // temporal value of sum of descendat sizes
        private int krSizesSumTmp = 0; // temporal value of sum of key roots sizes
        private int revkrSizesSumTmp = 0; // temporal value of sum of reversed hey roots sizes
        private int preorderTmp = 0; // temporal value of preorder

        // remembers what is the current node's postorder (current in the TED recursion)
        private int currentNode = -1;

        // remembers if the trees order was switched during the recursion (in comparison with the order of input trees)
        private bool switched = false;

        // as the names say
        private int leafCount = 0;
        private int treeSize = 0;

        public static void main(string[] args) {

        }

        /**
         * Creates an InfoTree object, gathers all information about aInputTree and stores in indexes.
         * aInputTree is not needed any more.
         * Remember to pass the same LabelDictionary object to both trees which are compared.
         * 
         * @param aInputTree an LblTree object
         * @param aLd  a LabelDictionary object
         */
        public InfoTree(LblTree aInputTree, LabelDictionary aLd) {
            this.inputTree = aInputTree;
            treeSize = inputTree.getNodeCount();

            this.info = Arrays.Allocate<int>(16, treeSize);
            Arrays.Fill(info[POST2_PARENT], -1);
            Arrays.Fill(info[POST2_MIN_KR], -1);
            Arrays.Fill(info[RPOST2_MIN_RKR], -1);
            Arrays.Fill(info[POST2_STRATEGY], -1);

            this.paths = Arrays.Allocate<int>(3, treeSize);
            Arrays.Fill(paths[LEFT], -1);
            Arrays.Fill(paths[RIGHT], -1);
            Arrays.Fill(paths[HEAVY], -1);

            this.relSubtrees = new int[3][][];
            for (var i = 0; i < this.relSubtrees.Length; i++)
            {
                relSubtrees[i] = new int[treeSize][];
            }

            this.nodeType = Arrays.Allocate<bool>(3, treeSize);
            this.ld = aLd;
            this.currentNode = treeSize - 1;
            gatherInfo(inputTree, -1);
            postTraversalProcessing();
        }

        /**
         * Returns the size of the tree.
         * 
         * @return
         */
        public int getSize() {
            return treeSize;
        }

        public bool ifNodeOfType(int postorder, int type) {
            return nodeType[type][postorder];
        }

        public bool[] getNodeTypeArray(int type) {
            return nodeType[type];
        }


        /**
         * For given infoCode and postorder of a node returns requested information of that node.
         * 
         * @param infoCode
         * @param nodesPostorder postorder of a node
         * @return a value of requested information
         */
        public int getInfo(int infoCode, int nodesPostorder) {
            // return info under infoCode and nodesPostorder
            return info[infoCode][nodesPostorder];
        }

        /**
         * For given infoCode returns an info array (index array)
         * 
         * @param infoCode
         * @return array with requested index
         */
        public int[] getInfoArray(int infoCode) {
            return info[infoCode];
        }

        /**
         * Returns relevant subtrees for given node. Assuming that child v of given node belongs to given path, 
         * all children of given node are returned but node v.
         * 
         * @param pathType
         * @param nodePostorder postorder of a node
         * @return  an array with relevant subtrees of a given node
         */
        public int[] getNodeRelSubtrees(int pathType, int nodePostorder) {
            return relSubtrees[pathType][nodePostorder];
        }

        /**
         * Returns an array representation of a given path's type.
         * 
         * @param pathType
         * @return an array with a requested path
         */
        public int[] getPath(int pathType) {
            return paths[pathType];
        }

        /**
         * Returns the postorder of current root node.
         * 
         * @return 
         */
        public int getCurrentNode() {
            return currentNode;
        }

        /**
         * Sets postorder of the current node in the recursion.
         * 
         * @param postorder 
         */
        public void setCurrentNode(int postorder) {
            currentNode = postorder;
        }

        /**
         * Gathers information of a given tree in corresponding arrays.
         * 
         * At this point the given tree is traversed once, but there is a loop over current nodes children
         * to assign them their parents.
         * 
         * @param aT
         * @param postorder
         * @return 
         */
        private int gatherInfo(LblTree aT, int postorder) {
            int currentSize = 0;
            int childrenCount = 0;
            int descSizes = 0;
            int krSizesSum = 0;
            int revkrSizesSum = 0;
            int preorder = preorderTmp;

            int heavyChild = -1;
            int leftChild = -1;
            int rightChild = -1;
            int weight = -1;
            int maxWeight = -1;
            int currentPostorder = -1;
            int oldHeavyChild = -1;

            var heavyRelSubtreesTmp = new List<int>();
            var leftRelSubtreesTmp = new List<int>();
            var rightRelSubtreesTmp = new List<int>();

            var childrenPostorders = new List<int>();

            preorderTmp++;

            var e = new TreeNodeIterator { Pointer = aT, ChildrenIndex = 0 };
       
            // enumerate over children of current node
            while (e.Pointer.Children.Count > e.ChildrenIndex) {
                childrenCount++;

                postorder = gatherInfo((LblTree)e.Pointer.Children[e.ChildrenIndex], postorder);
                e.ChildrenIndex++;

                childrenPostorders.Add(postorder);

                currentPostorder = postorder;

                // heavy path
                weight = sizeTmp + 1;
                if (weight >= maxWeight) {
                    maxWeight = weight;
                    oldHeavyChild = heavyChild;
                    heavyChild = currentPostorder;
                } else {
                    heavyRelSubtreesTmp.Add(currentPostorder);
                }
                if (oldHeavyChild != -1) {
                    heavyRelSubtreesTmp.Add(oldHeavyChild);
                    oldHeavyChild = -1;
                }

                // left path
                if (childrenCount == 1) {
                    leftChild = currentPostorder;
                } else {
                    leftRelSubtreesTmp.Add(currentPostorder);
                }

                // right path
                rightChild = currentPostorder;
                if (e.Pointer.Children.Count > e.ChildrenIndex) {
                    rightRelSubtreesTmp.Add(currentPostorder);
                }

                // subtree size
                currentSize += 1 + sizeTmp;

                descSizes += descSizesTmp;

                if (childrenCount > 1) {
                    krSizesSum += krSizesSumTmp + sizeTmp + 1;
                } else {
                    krSizesSum += krSizesSumTmp;
                    nodeType[LEFT][currentPostorder] = true;
                }

                if (e.Pointer.Children.Count > e.ChildrenIndex) {
                    revkrSizesSum += revkrSizesSumTmp + sizeTmp + 1;
                } else {
                    revkrSizesSum += revkrSizesSumTmp;
                    nodeType[RIGHT][currentPostorder] = true;
                }
            }

            postorder++;

            // postorder
            aT.setTmpData(postorder);

            int currentDescSizes = descSizes + currentSize + 1;
            info[POST2_DESC_SUM][postorder] = (currentSize + 1) * (currentSize + 1 + 3) / 2 - currentDescSizes;
            info[POST2_KR_SUM][postorder] = krSizesSum + currentSize + 1;
            info[POST2_REV_KR_SUM][postorder] = revkrSizesSum + currentSize + 1;

            // POST2_LABEL
            //labels[rootNumber] = ld.store(aT.getLabel());
            info[POST2_LABEL][postorder] = ld.store(aT.getLabel());

            // POST2_PARENT
            foreach (var i in childrenPostorders) {
                info[POST2_PARENT][i] = postorder;
            }

            // POST2_SIZE
            info[POST2_SIZE][postorder] = currentSize + 1;
            if (currentSize == 0) {
                leafCount++;
            }

            // POST2_PRE
            info[POST2_PRE][postorder] = preorder;

            // PRE2_POST
            info[PRE2_POST][preorder] = postorder;

            // RPOST2_POST
            info[RPOST2_POST][treeSize - 1 - preorder] = postorder;

            // heavy path
            if (heavyChild != -1) {
                paths[HEAVY][postorder] = heavyChild;
                nodeType[HEAVY][heavyChild] = true;

                if (leftChild < heavyChild && heavyChild < rightChild) {
                    info[POST2_STRATEGY][postorder] = BOTH;
                } else if (heavyChild == leftChild) {
                    info[POST2_STRATEGY][postorder] = RIGHT;
                } else if (heavyChild == rightChild) {
                    info[POST2_STRATEGY][postorder] = LEFT;
                }
            } else {
                info[POST2_STRATEGY][postorder] = RIGHT;
            }



            // left path
            if (leftChild != -1) {
                paths[LEFT][postorder] = leftChild;
            }
            // right path
            if (rightChild != -1) {
                paths[RIGHT][postorder] = rightChild;
            }

            // heavy/left/right relevant subtrees
            relSubtrees[HEAVY][postorder] = heavyRelSubtreesTmp.ToArray();
            relSubtrees[RIGHT][postorder] = rightRelSubtreesTmp.ToArray();
            relSubtrees[LEFT][postorder] = leftRelSubtreesTmp.ToArray();

            descSizesTmp = currentDescSizes;
            sizeTmp = currentSize;
            krSizesSumTmp = krSizesSum;
            revkrSizesSumTmp = revkrSizesSum;

            return postorder;
        }

        /**
         * Gathers information, that couldn't be collected while tree traversal.
         */
        private void postTraversalProcessing() {
            int nc1 = treeSize;
            info[KR] = new int[leafCount];
            info[RKR] = new int[leafCount];

            int nc = nc1;
            int lc = leafCount;
            int i = 0;

            // compute left-most leaf descendants
            // go along the left-most path, remember each node and assign to it the path's leaf
            // compute right-most leaf descendants (in reversed postorder)
            for (i = 0; i < treeSize; i++) {
                if (paths[LEFT][i] == -1) {
                    info[POST2_LLD][i] = i;
                } else {
                    info[POST2_LLD][i] = info[POST2_LLD][paths[LEFT][i]];
                }
                if (paths[RIGHT][i] == -1) {
                    info[RPOST2_RLD][treeSize - 1 - info[POST2_PRE][i]] = (treeSize - 1 - info[POST2_PRE][i]);
                } else {
                    info[RPOST2_RLD][treeSize - 1 - info[POST2_PRE][i]] = info[RPOST2_RLD][treeSize - 1 - info[POST2_PRE][paths[RIGHT][i]]];
                }
            }

            // compute key root nodes
            // compute reversed key root nodes (in revrsed postorder)
            bool[] visited = new bool[nc];
            bool[] visitedR = new bool[nc];
            Arrays.Fill(visited, false);
            int k = lc - 1;
            int kR = lc - 1;
            for (i = nc - 1; i >= 0; i--) {
                if (!visited[info[POST2_LLD][i]]) {
                    info[KR][k] = i;
                    visited[info[POST2_LLD][i]] = true;
                    k--;
                }
                if (!visitedR[info[RPOST2_RLD][i]]) {
                    info[RKR][kR] = i;
                    visitedR[info[RPOST2_RLD][i]] = true;
                    kR--;
                }
            }

            // compute minimal key roots for every subtree
            // compute minimal reversed  key roots for every subtree (in reversed postorder)
            int parent = -1;
            int parentR = -1;
            for (i = 0; i < leafCount; i++) {
                parent = info[KR][i];
                while (parent > -1 && info[POST2_MIN_KR][parent] == -1) {
                    info[POST2_MIN_KR][parent] = i;
                    parent = info[POST2_PARENT][parent];
                }
                parentR = info[RKR][i];
                while (parentR > -1 && info[RPOST2_MIN_RKR][parentR] == -1) {
                    info[RPOST2_MIN_RKR][parentR] = i;
                    parentR = info[POST2_PARENT][info[RPOST2_POST][parentR]]; // get parent's postorder
                    if (parentR > -1) {
                        parentR = treeSize - 1 - info[POST2_PRE][parentR]; // if parent exists get its rev. postorder
                    }
                }
            }
        }

        public void setSwitched(bool value) {
            switched = value;
        }
        public bool isSwitched() {
            return switched;
        }
    }
}