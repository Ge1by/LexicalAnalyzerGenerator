using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB5
{
    // Класс "Генератор".
    class Generator
    {
        SyntaxTreeNode treeRoot; // Корень синтаксического дерева.
        List<string> outputText; // Выходной текст - список строк.  

        // Конструктор генератора.
        // В качестве параметра поступает корень синтаксического дерева.
        public Generator(SyntaxTreeNode treeRoot)
        {
            this.treeRoot = treeRoot;
            outputText = new List<string>();
        }

        // Выходной текст - свойство только для чтения.
        public List<string> OutputText
        {
            get { return outputText; }
        }

        // Сгенерировать структурированный текст.
        public void GenerateStructuredText()
        {
            outputText.Clear(); // Очищаем выходной текст.
            RecurTraverseTree(treeRoot, 0); // Рекурсивно обходим дерево и формируем выходной текст.
        }

        // Рекурсивно обойти дерево, формируя выходной текст.
        // node - узел дерева.
        // indent - отступ.
        private void RecurTraverseTree(SyntaxTreeNode node, int indent)
        {
            if (node.SubNodes.Count() > 0) // Если текущий узел - нетерминал.
            {
                foreach(SyntaxTreeNode item in node.SubNodes) // Цикл по всем подчиненным узлам.
                {
                        RecurTraverseTree(item, indent + 10); // Значит, это выражение в скобках. Выведем его с дополнительным отступом.
                }
            }
            else
            {
                string s = "";

                // Генерируем отступ.       
                for(int i = 0; i < indent; i++)
                {
                    s += " ";
                }

                if (node.Name != "X" && node.Name != "C") 
                {
                    if (node.Token.Type == TokenKind.Number)
                    {
                    s += Convert.ToInt64(node.Name,2);
                    }
                    else
                    // Добавляем имя узла.
                    s += node.Name;
                
                }

                // Добавляем созданную строку в результат.
                outputText.Add(s);
            }
        }
    }
}
