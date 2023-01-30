using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LAB5
{
    // Класс исключительных ситуаций синтаксического анализа.
    class SynAnException : Exception
    {
        // Позиция возникновения исключительной ситуации в анализируемом тексте.
        private int lineIndex; // Индекс строки.
        private int symIndex;  // Индекс символа.

        // Индекс строки, где возникла исключительная ситуация - свойство только для чтения.
        public int LineIndex
        {
            get { return lineIndex; }
        }

        // Индекс символа, на котором возникла исключительная ситуация - свойство только для чтения.
        public int SymIndex
        {
            get { return symIndex; }
        }

        // Конструктор исключительной ситуации.
        // message - описание исключительной ситуации.
        // lineIndex и symIndex - позиция возникновения исключительной ситуации в анализируемом тексте.
        public SynAnException(string message, int lineIndex, int symIndex)
            : base(message)
        {
            this.lineIndex = lineIndex;
            this.symIndex = symIndex;
        }
    }
    class ContextAnException : Exception
    {
        // Позиция возникновения исключительной ситуации в анализируемом тексте.
        private int lineIndex; // Индекс строки.
        private int symIndex;  // Индекс символа.

        // Индекс строки, где возникла исключительная ситуация - свойство только для чтения.
        public int LineIndex
        {
            get { return lineIndex; }
        }

        // Индекс символа, на котором возникла исключительная ситуация - свойство только для чтения.
        public int SymIndex
        {
            get { return symIndex; }
        }

        // Конструктор исключительной ситуации.
        // message - описание исключительной ситуации.
        // lineIndex и symIndex - позиция возникновения исключительной ситуации в анализируемом тексте.
        public ContextAnException(string message, int lineIndex, int symIndex)
            : base(message)
        {
            this.lineIndex = lineIndex;
            this.symIndex = symIndex;
        }
    }

    // Класс "Синтаксический анализатор".
    // При обнаружении ошибки в исходном тексте он генерирует исключительную ситуацию SynAnException или LexAnException.
    class SyntaxAnalyzer
    {
        private LexicalAnalyzer lexAn; // Лексический анализатор.

        // Конструктор синтаксического анализатора. 
        // В качестве параметра передается исходный текст.
        public SyntaxAnalyzer(string[] inputLines)
        {
            // Создаем лексический анализатор.
            // Передаем ему текст.
            lexAn = new LexicalAnalyzer(inputLines);
        }

        // Обработать синтаксическую ошибку.
        // msg - описание ошибки.
        private void SyntaxError(string msg)
        {
            // Генерируем исключительную ситуацию, тем самым полностью прерывая процесс анализа текста.
            throw new SynAnException(msg, lexAn.CurLineIndex, lexAn.CurSymIndex);
        }
        private void ContextError(string msg)
        {
            // Генерируем исключительную ситуацию, тем самым полностью прерывая процесс анализа текста.
            throw new ContextAnException(msg, lexAn.CurLineIndex, lexAn.CurSymIndex);
        }


        // Проверить, что тип текущего распознанного токена совпадает с заданным.
        // Если совпадает, то распознать следующий токен, иначе синтаксическая ошибка.
        private void Match(TokenKind tkn)
        {
            if (lexAn.Token.Type == tkn) // Сравниваем.
            {
                lexAn.RecognizeNextToken(); // Распознаем следующий токен.
            }
            else
            {
                SyntaxError("Ожидалось " + tkn.ToString()); // Обнаружена синтаксическая ошибка.
            }
        }


        // Проверить, что текущий распознанный токен совпадает с заданным (сравнение производится в нижнем регистре).
        // Если совпадает, то распознать следующий токен, иначе синтаксическая ошибка.
        private void Match(string tkn)
        {
            if (lexAn.Token.Value.ToLower() == tkn.ToLower()) // Сравниваем.
            {
                lexAn.RecognizeNextToken(); // Распознаем следующий токен.
            }
            else
            {
                SyntaxError("Ожидалось " + tkn); // Обнаружена синтаксическая ошибка.
            }
        }

        // Процедура разбора для стартового нетерминала S.
        private void S(out SyntaxTreeNode node, out Dictionary<int, int> id)
        {
            node = new SyntaxTreeNode("S");
            id = new Dictionary<int, int>();

            SyntaxTreeNode nodeA;
            A(out nodeA, id); // Вызываем процедуру разбора для нетерминала A.
            node.AddSubNode(nodeA); // К узлу с именем "S" добавляем подчиненный узел, созданный в A().

            SyntaxTreeNode nodeC;
            C(out nodeC, id); // Вызываем процедуру разбора для нетерминала S.
            node.AddSubNode(nodeC); // К узлу с именем "S" добавляем подчиненный узел, созданный в C().
        }

        // Процедура разбора для нетерминала C.
        private void C(out SyntaxTreeNode node, Dictionary<int, int> id)
        {
            node = new SyntaxTreeNode("C");
            if (lexAn.Token.Type == TokenKind.Semicolon) // Если текущий токен - ";".
            {
                node.AddSubNode(new SyntaxTreeNode(lexAn.Token));
                lexAn.RecognizeNextToken(); // Пропускаем этот знак.

                SyntaxTreeNode nodeS;
                S(out nodeS, out id); // Вызываем процедуру разбора для нетерминала S.
                node.AddSubNode(nodeS); // К узлу с именем "C" добавляем подчиненный узел, созданный в S().

                SyntaxTreeNode nodeC;
                C(out nodeC, id); // Вызываем процедуру разбора для нетерминала S.
                node.AddSubNode(nodeC); // К узлу с именем "C" добавляем подчиненный узел, созданный в C().
            }
            else
            {
                //Епсилон
            }
        }

        // Процедура разбора для нетерминала A.
        private void A(out SyntaxTreeNode node, Dictionary<int, int> id)
        {
            node = new SyntaxTreeNode("A");

            SyntaxTreeNode nodeB;
            B(out nodeB, id); // Вызываем процедуру разбора для нетерминала B.
            node.AddSubNode(nodeB); // К узлу с именем "A" добавляем подчиненный узел, созданный в B().

            SyntaxTreeNode nodeX;
            X(out nodeX, id); // Вызываем процедуру разбора для нетерминала X.
            node.AddSubNode(nodeX); // К узлу с именем "A" добавляем подчиненный узел, созданный в X().
        }

        // Процедура разбора для нетерминала X.
        private void X(out SyntaxTreeNode node, Dictionary<int, int> id)
        {
            node = new SyntaxTreeNode("X");

            if (lexAn.Token.Type == TokenKind.Comma) // Если текущий токен - ",".
            {
                node.AddSubNode(new SyntaxTreeNode(lexAn.Token));
                lexAn.RecognizeNextToken(); // Пропускаем этот знак.

                SyntaxTreeNode nodeB;
                B(out nodeB, id); // Вызываем процедуру разбора для нетерминала B.
                node.AddSubNode(nodeB); // К узлу с именем "A" добавляем подчиненный узел, созданный в B().

                SyntaxTreeNode nodeX;
                X(out nodeX, id); // Вызываем процедуру разбора для нетерминала X.
                node.AddSubNode(nodeX); // К узлу с именем "A" добавляем подчиненный узел, созданный в X().
            }
            else
            {
                //Епсилон
            }
        }

        // Процедура разбора для нетерминала B.
        private void B(out SyntaxTreeNode node, Dictionary<int, int> id)
        {
            node = new SyntaxTreeNode("B");

            if (lexAn.Token.Type == TokenKind.Number)
            {
                node.AddSubNode(new SyntaxTreeNode(lexAn.Token));
                if (!id.ContainsKey(Convert.ToInt32(lexAn.Token.Value, 2)))
                    id.Add(Convert.ToInt32(lexAn.Token.Value, 2), 1);
                else
                    ContextError("Произошло объявление числа, которое было объявлено ранее.");
                Match(TokenKind.Number);
            }
            else if (lexAn.Token.Type == TokenKind.Word)
            {
                node.AddSubNode(new SyntaxTreeNode(lexAn.Token));
                Match(TokenKind.Word);

                node.AddSubNode(new SyntaxTreeNode(lexAn.Token));
                Match(TokenKind.Colon);

                node.AddSubNode(new SyntaxTreeNode(lexAn.Token));
                if (!id.ContainsKey(Convert.ToInt32(lexAn.Token.Value, 2)))
                    id.Add(Convert.ToInt32(lexAn.Token.Value, 2), 1);
                else
                    ContextError("Произошло объявление числа, которое было объявлено ранее.");
                Match(TokenKind.Number);
            }
        }



        // Провести синтаксический анализ текста.
        // Возвращает корень синтаксического дерева.
        // Дерево строится в порядке "Снизу-Вверх, Слева-Направо" на основе применения синтезируемых атрибутов.
        public void ParseText(out SyntaxTreeNode treeRoot)
        {
            lexAn.RecognizeNextToken(); // Распознаем первый токен в тексте.
            Dictionary<int, int> id;

            S(out treeRoot, out id); // Вызываем процедуру разбора для стартового нетерминала E. 

            if (lexAn.Token.Type != TokenKind.EndOfText) // Если текущий токен не является концом текста.
            {
                SyntaxError("После арифметического выражения идет еще какой-то текст"); // Обнаружена синтаксическая ошибка.
            }
        }
    }
}
