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

using System;

namespace Barbar.TreeDistance.Util
{

    /**
     * A node of a tree. Each tree has an ID.
     * The label can be empty, but can not contain trailing spaces (nor consist only of spaces).
     * 
     * Two nodes are equal, if there labels are equal, and n1 < n2 if label(n1) < label(n2).
     * 
     * @author Nikolaus Augsten from approxlib, available at http://www.inf.unibz.it/~augsten/src/ modified by Mateusz Pawlik
     */
    public class LblTree : TreeNode, IComparable<LblTree> {

        public const string TAB_STRING = "    ";
        public const string ROOT_STRING =   "*---+";
        public const string BRANCH_STRING = "+---+";

        public const string OPEN_BRACKET = "{";
        public const string CLOSE_BRACKET = "}";
        public const string ID_SEPARATOR = ":";

        public const int HIDE_NOTHING = 0;
        public const int HIDE_ROOT_LABEL = 1;
        public const int RENAME_LABELS_TO_LEVEL = 2;
        public const int HIDE_ALL_LABELS = 3;
        public const int RANDOM_ROOT_LABEL = 4;

        /**
	 * no node id
	 */
        public const int NO_NODE = -1;

        /**
         * no tree id is defined
         */
        public const int NO_TREE_ID = -1;
	
	int treeID = NO_TREE_ID;
        string label = null;
        Object tmpData = null;
	int nodeID = NO_NODE;

        /**
         * Use only this constructor!
         */
        public LblTree(string label, int treeID) {
            this.treeID = treeID;
            this.label = label;
        }

        public void setLabel(string label) {
            this.label = label;
        }

        public string getLabel() {
            return label;
        }

        public int getTreeID() {
            if (isRoot()) {
                return treeID;
            } else {
                return ((LblTree)getRoot()).getTreeID();
            }
        }

        public void setTreeID(int treeID) {
            if (isRoot()) {
                this.treeID = treeID;
            } else {
                ((LblTree)getRoot()).setTreeID(treeID);
            }
        }

        /**
         * tmpData: Hook for any data that a method must attach to a tree.
         * Methods can assume, that this date is null and should return it
         * to be null!
         */
        public void setTmpData(Object tmpData) {
            this.tmpData = tmpData;
        }

        public Object getTmpData() {
            return tmpData;
        }

        public void prettyPrint() {
            prettyPrint(false);
        }


        public void prettyPrint(bool printTmpData) {
            for (int i = 0; i < getLevel(); i++) {
                Console.Out.Write(TAB_STRING);
            }
            if (!isRoot()) {
                Console.Out.Write(BRANCH_STRING);
            } else {
                if (getTreeID() != NO_TREE_ID) {
                    Console.Out.WriteLine("treeID: " + getTreeID());
                }
                Console.Out.Write(ROOT_STRING);
            }
            Console.Out.Write(" '" + this.getLabel() + "' ");
            if (printTmpData) {
                Console.Out.WriteLine(getTmpData());
            } else {
                Console.Out.WriteLine();
            }
            foreach(var child in Children) {
                ((LblTree)child).prettyPrint(printTmpData);
            }

        }

        public int getNodeCount() {
            int sum = 1;
            foreach (var child in Children) {
                sum += ((LblTree)child).getNodeCount();
            }
            return sum;
        }


        /**
         * Constructs an LblTree from a string representation of tree. The
         * treeID in the string representation is optional; if no treeID is given,
         * the treeID of the returned tree will be NO_ID.
         *
         * @param s string representation of a tree. Format: "treeID:{root{...}}".
         * @return tree represented by s
         */
        public static LblTree fromString(string s) {
            int treeID = FormatUtilities.getTreeID(s);
            s = s.JavaSubstring(s.IndexOf(OPEN_BRACKET), s.LastIndexOf(CLOSE_BRACKET) + 1);
            LblTree node = new LblTree(FormatUtilities.getRoot(s), treeID);
            var c = FormatUtilities.getChildren(s);
            for (int i = 0; i < c.Count; i++) {
                node.Add(fromString(c[i]));
            }
            return node;
        }

        /**
         * string representation of a tree. Reverse operation of {@link #fromString(string)}.
         * treeID is NO_ID, it is skiped in the string representation.
         *
         * @return string representation of this tree
         *
         */
      public override string ToString() {
            string res = OPEN_BRACKET + getLabel();
            if ((getTreeID() >= 0) && (isRoot())) {
                res = getTreeID() + ID_SEPARATOR + res;
            }
            foreach (var child in Children) {
                res += child.ToString();
            }
            res += CLOSE_BRACKET;
            return res;
        }

        /**
         * Clear tmpData in subtree rooted in this node.
         */
        //public void clearTmpData() {
          //  for (Enumeration e = breadthFirstEnumeration(); e.hasMoreElements();) {
            //    ((LblTree)e.nextElement()).setTmpData(null);
            //}
        //}

        /**
         * Compares the labels.
         */
        public int CompareTo(LblTree other)
        {
            return getLabel().CompareTo(other.getLabel());
        }
    }
}