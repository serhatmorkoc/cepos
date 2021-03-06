using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Hugin.POS.Common;
using Hugin.POS.Data;

namespace Hugin.POS.Display
{
    enum AdjustmentFactor
    {
        Percentage,
        Amount
    }

    enum WindowType
    {
        PluList,
        PluPage,
        Program,
        Payment,
        Function,
        Input
    }

    public class Skin
    {
        public const int MAX_CHAR_IN_BUTTON = 9;
#if ORDER
        public const int NUMBER_OF_PLU_ON_PAGE = 20;
        public const int MAX_BUTTON_IN_LINE = 6;
#else
        public const int MAX_BUTTON_IN_LINE = 6;
        public const int NUMBER_OF_PLU_ON_PAGE = 17;
#endif



        private String text;
        private String name;
        private Point cellInfo;
        private Color startColor = Colors.DEF_START_COLOR;
        private Color endColor = Colors.DEF_END_COLOR;
        private Color foreColor = Colors.MenuText;
        private int colSpan = 1;
        private int rowSpan=1;
        private Font font = new System.Drawing.Font("Tahoma", 20F, FontStyle.Regular);

        private static int pluPageNo = 1;

        private static List<Skin> functionSkinList = null;
        private static List<Skin> programSkinList = null;
        private static List<Skin> paymentSkinList = null;
        private static List<Skin> pageSkinList = null;
        private static List<Skin> pluSkinList = null;
        private static List<Skin> qwertySkinList = null;

        private static Font DefaultFont = new System.Drawing.Font("Tahoma", 17F, FontStyle.Bold);
        private static Font NumericKeyFont = new System.Drawing.Font("Tahoma", 33F, FontStyle.Bold);
        private static Font KeyboardFont = new System.Drawing.Font("Tahoma", 36F, FontStyle.Bold);
        private static Font DefaultPluFont = new System.Drawing.Font("Tahoma", 15F, FontStyle.Bold);
                                           
                                                    
#region Properties
        public Font Font
        {
            get { return font; }
        }

        public Color StartColor
        {
            get { return startColor; }
        }

        public Color EndColor
        {
            get { return endColor; }
        }

        public Color ForeColor
        {
            get { return foreColor; }
        }

        public String Name
        {
            get { return name; }
        }

        public String Text
        {
            get { return text; }
        }

        public Point CellInfo
        {
            get { return cellInfo; }
        }

        public int ColSpan
        {
            get { return colSpan; }
        }

        public int RowSpan
        {
            get { return rowSpan; }
        }

        public static int PLUPageNo
        {
            get
            {
                return pluPageNo;
            }
            set
            {
                if (pluPageNo != value)
                {
                    pluSkinList = null;
                }
                pluPageNo = value;
            }
        }
        

#endregion

        public Skin(string name, string text)
        {
            this.name = name;
            this.text = text;
        }

        public Skin(string name, string text, Point cellInfo, Font font, Color startColor, Color endColor)
            : this(name, text)
        {
            this.font = font;
            this.cellInfo = cellInfo;
            this.startColor = startColor;
            this.endColor = endColor;
        }

        public Skin(string name, string text, Point cellInfo, Font font, Color startColor, Color endColor, Color foreColor)
            : this(name, text, cellInfo, font, startColor, endColor)
        {
            this.foreColor = foreColor;
        }


        private static List<Skin> GetProgramSkinList()
        {
            if (programSkinList != null)
                return programSkinList;

            programSkinList = new List<Skin>();

#if ORDER
            int row = 4;
            int col = 3;
            programSkinList.Add(new Skin("btnDocument", PosMessage.DOCUMENT, new Point(col++, row), DefaultFont, Colors.PriceStart, Colors.PriceEnd));
            programSkinList.Add(new Skin("btnEmpty", "", new Point(col++, row), DefaultFont, Colors.PLUStart, Colors.PLUEnd));
            col = 4;
            row = 5;
            programSkinList.Add(new Skin("btnCustomer", PosMessage.CUSTOMER, new Point(col++, row), DefaultFont, Colors.PaymentStart, Colors.PaymentEnd));
            col = 3;
            row = 6;
            programSkinList.Add(new Skin("btnProgram", PosMessage.PROGRAM, new Point(col++, row), DefaultFont, Colors.CommandStart, Colors.CommandEnd));
            programSkinList.Add(new Skin("btnClerk", PosMessage.CLERK, new Point(col++, row), DefaultFont, Colors.PaymentStart, Colors.PaymentEnd));
            row = 7;
            col = 4;
            programSkinList.Add(new Skin("btnDrawer", PosMessage.CASH_DRAWER, new Point(col++, row), DefaultFont, Colors.InOrder_DrawerStart, Colors.InOrder_DrawerEnd));
#else

            int row = 3;
            int col = 1;
            programSkinList.Add(new Skin("btnPayOut", PosMessage.PAYOUT, new Point(col++, row), DefaultFont, Colors.AdjustmentStart, Colors.AdjustmentEnd));
            programSkinList.Add(new Skin("btnReceiveOnAcct", PosMessage.RECEIVE_ACCT_ON_PAYMENT, new Point(col++, row), DefaultFont, Colors.AdjustmentStart, Colors.AdjustmentEnd));
            col++;
            programSkinList.Add(new Skin("btnDocument", PosMessage.DOCUMENT, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
            col = 4;
            row++;
            programSkinList.Add(new Skin("btnClerk", PosMessage.CLERK, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
            col = 4;
            row++;
            programSkinList.Add(new Skin("btnReport", PosMessage.REPORT, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
            col = 4;
            row++;
            programSkinList.Add(new Skin("btnProgram", PosMessage.PROGRAM, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
            col = 3;
            row++;
            programSkinList.Add(new Skin("btnCustomer", PosMessage.CUSTOMER, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
            programSkinList.Add(new Skin("btnDrawer", PosMessage.CASH_DRAWER, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
#endif

            return programSkinList;
        }

        private static List<Skin> GetFunctionSkins()
        {
            if (functionSkinList != null)
                return functionSkinList;

            functionSkinList = new List<Skin>();
            int row = 3;
            int col = 0;
            Point point = new Point(col, row);
#if ORDER
            functionSkinList.Add(new Skin("btnQuantity", PosMessage.MULTIPLY, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
            functionSkinList.Add(new Skin("btnAmount", PosMessage.AMOUNT, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
            functionSkinList.Add(new Skin("btnPrice", PosMessage.PRICE, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
            col = 0;
            row++;
            functionSkinList.Add(new Skin("btnD7", "7", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD8", "8", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD9", "9", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));


            //functionSkinList.Add(new Skin("btnSubtotal", PosMessage.SUBTOTAL, new Point(4, 4), DefaultFont, Colors.ExecuteStart, Colors.ExecuteEnd));

            functionSkinList.Add(new Skin("btnEscape", PosMessage.ESCAPE, new Point(5, 4), DefaultFont, Colors.ExecuteStart, Colors.ExecuteEnd));

            col = 0;
            row++;
            functionSkinList.Add(new Skin("btnD4", "4", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD5", "5", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD6", "6", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnCommand", PosMessage.COMMAND, new Point(col++, row), DefaultFont, Colors.CommandStart, Colors.CommandEnd));
            
            col = 5;
            row = 5;
            functionSkinList.Add(new Skin("btnUpArrow", "↑\n", new Point(col++, row), NumericKeyFont, Colors.ExecuteStart, Colors.ExecuteEnd));

            col = 0;
            row++;
            functionSkinList.Add(new Skin("btnD1", "1", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD2", "2", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD3", "3", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            row = 6;
            col = 5;
            functionSkinList.Add(new Skin("btnDownArrow", "↓\n", new Point(col++, row), NumericKeyFont, Colors.ExecuteStart, Colors.ExecuteEnd));
            col = 0;
            row++;
            functionSkinList.Add(new Skin("btnCorrection", "←\n", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD0", "0", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnSeperator", ",", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnPriceLookup", PosMessage.PRICE_LOOKUP, new Point(col++, row), DefaultFont, Colors.InOrder_PriceLookUpStart, Colors.InOrder_PriceLookUpEnd));
            row = 7;
            col = 5;
            functionSkinList.Add(new Skin("btnEnter", PosMessage.ENTER, new Point(col++, row), DefaultFont, Colors.ExecuteStart, Colors.ExecuteEnd));
#else
            functionSkinList.Add(new Skin("btnQuantity", PosMessage.MULTIPLY, new Point(col++, row), DefaultFont, Colors.ConfirmStart, Colors.ConfirmEnd));
            col += 1;
            col += 1;
            functionSkinList.Add(new Skin("btnPriceLookup", PosMessage.PRICE_LOOKUP, new Point(col++, row), DefaultFont, Colors.PriceStart, Colors.PriceEnd));
            functionSkinList.Add(new Skin("btnDocument", PosMessage.DOCUMENT, new Point(col++, row), DefaultFont, Colors.MenuStart, Colors.MenuEnd));
            functionSkinList.Add(new Skin("btnEscape", PosMessage.ESCAPE, new Point(col++, row), DefaultFont, Colors.ExecuteStart, Colors.ExecuteEnd));
            col = 0;
            row++;
            functionSkinList.Add(new Skin("btnD7", "7", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD8", "8", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD9", "9", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnAmount", PosMessage.AMOUNT, new Point(col++, row), DefaultFont, Colors.PriceStart, Colors.PriceEnd));
            col += 1;
            functionSkinList.Add(new Skin("btnUpArrow", "↑\n", new Point(col++, row), NumericKeyFont, Colors.ExecuteStart, Colors.ExecuteEnd));
            col = 0;
            row++;
            functionSkinList.Add(new Skin("btnD4", "4", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD5", "5", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD6", "6", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnPrice", PosMessage.PRICE, new Point(col++, row), DefaultFont, Colors.PriceStart, Colors.PriceEnd));
            col += 1;
            functionSkinList.Add(new Skin("btnDownArrow", "↓\n", new Point(col++, row), NumericKeyFont, Colors.ExecuteStart, Colors.ExecuteEnd));
            col = 0;
            row++;
            functionSkinList.Add(new Skin("btnD1", "1", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD2", "2", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD3", "3", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnCommand", PosMessage.COMMAND, new Point(col++, row), DefaultFont, Colors.CommandStart, Colors.CommandEnd));
            col += 1;
            functionSkinList.Add(new Skin("btnSubtotal", PosMessage.SUBTOTAL, new Point(col++, row), DefaultFont, Colors.ExecuteStart, Colors.ExecuteEnd));
            col = 0;
            row++;
            functionSkinList.Add(new Skin("btnCorrection", "←\n", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnD0", "0", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            functionSkinList.Add(new Skin("btnSeperator", ",", new Point(col++, row), NumericKeyFont, Colors.NumericStart, Colors.NumericEnd, Colors.NumericText));
            col += 1;
            col += 1;
            functionSkinList.Add(new Skin("btnEnter", PosMessage.ENTER, new Point(col++, row), DefaultFont, Colors.ExecuteStart, Colors.ExecuteEnd));
#endif

            return functionSkinList;
        }

        private static List<Skin> GetPaymentSkinList()
        {
            if (paymentSkinList != null)
                return paymentSkinList;

            paymentSkinList = new List<Skin>();

#if ORDER
            int row = 4;
            int col = 3;
            paymentSkinList.Add(new Skin("btnDiscount", "-%", new Point(col++, row), DefaultFont, Colors.AdjustmentStart, Colors.AdjustmentEnd));
            paymentSkinList.Add(new Skin("btnFee", "+%", new Point(col++, row), DefaultFont, Colors.AdjustmentStart, Colors.AdjustmentEnd));

            paymentSkinList.Add(new Skin("btnVoid", PosMessage.VOID, new Point(3, 6), DefaultFont, Colors.VoidStart, Colors.VoidEnd));

            paymentSkinList.Add(new Skin("btnSubtotal", PosMessage.SUBTOTAL, new Point(4, 6), DefaultFont, Colors.ExecuteStart, Colors.ExecuteEnd));

            paymentSkinList.Add(new Skin("btnSendOrder", PosMessage.SEND_ORDER, new Point(4, 7), DefaultFont, Colors.PaymentStart, Colors.PaymentEnd));
         

#else

            int row = 3;
            int col = 1;
            paymentSkinList.Add(new Skin("btnDiscount", "-%", new Point(col++, row), DefaultFont, Colors.AdjustmentStart, Colors.AdjustmentEnd));
            paymentSkinList.Add(new Skin("btnFee", "+%", new Point(col++, row), DefaultFont, Colors.AdjustmentStart, Colors.AdjustmentEnd));
            col++;
            paymentSkinList.Add(new Skin("btnVoid", PosMessage.VOID, new Point(col++, row), DefaultFont, Colors.VoidStart, Colors.VoidEnd));
            col = 4;
            row++;
            paymentSkinList.Add(new Skin("btnCredit", PosMessage.CREDIT, new Point(col++, row), DefaultFont, Colors.PaymentStart, Colors.PaymentEnd));
            col = 4;
            row++;
            string creditName = GetCreditName(1);
            paymentSkinList.Add(new Skin("btnCredit1", creditName, new Point(col++, row), DefaultFont, Colors.PaymentStart, Colors.PaymentEnd));
            col = 4;
            row++;
            creditName = GetCreditName(2);
            paymentSkinList.Add(new Skin("btnCredit2", creditName, new Point(col++, row), DefaultFont, Colors.PaymentStart, Colors.PaymentEnd));
            col = 3;
            row++;
            paymentSkinList.Add(new Skin("btnCheck", PosMessage.CHECK, new Point(col++, row), DefaultFont, Colors.PaymentStart, Colors.PaymentEnd));
            paymentSkinList.Add(new Skin("btnCash", PosMessage.CASH, new Point(col++, row), DefaultFont, Colors.PaymentStart, Colors.PaymentEnd));

#endif

            return paymentSkinList;
        }

        private static string GetCreditName(int creditNo)
        {
            return Connector.Instance().GetCredits()[creditNo].Name.Trim();
        }

        private static List<Skin> GetPLUPageSkinList()
        {
            if (pageSkinList != null)
                return pageSkinList;

            pageSkinList = new List<Skin>();

            int pagerCount = 6;
            int row = 2;
            int col = 0;

            for (int i = 0; i < pagerCount; i++)
            {
                pageSkinList.Add(new Skin("btnPage" + (i + 1), PosMessage.PLU_PAGE + " " + (i + 1), new Point(col++, row), DefaultPluFont, Colors.PageStart, Colors.PageEnd));
            }

            return pageSkinList;
        }

        private static List<Skin> GetPLUSkinList()
        {
            if (pluSkinList != null)
                return pluSkinList;

            String[,] pluNames = PluNameList();

            pluSkinList = new List<Skin>();

            int row = 0;
            int col = 0;
            Point pagerPoint = new Point(0, 2);
            for (int i = 0; i < NUMBER_OF_PLU_ON_PAGE; i++)
            {
                if (pagerPoint == new Point(col, row))
                {
                    pluSkinList.Add(new Skin("btnPager", (char)0xBB+"\n" + PosMessage.PLU_PAGE + " " + pluPageNo, pagerPoint, DefaultPluFont, Colors.PageStart, Colors.PageEnd));
                    col++;
                }
                pluSkinList.Add(new Skin("btnPLU" + (i + 1), pluNames[pluPageNo - 1, i], new Point(col, row), DefaultPluFont, Colors.PLUStart, Colors.PLUEnd));

                col++;
                if (col == MAX_BUTTON_IN_LINE)
                {
                    col = 0;
                    row++;

#if ORDER
                    if (row == 3)
                    {
                        col = 3;
                    }
#endif
                }

            }


            return pluSkinList;
        }

        private static string[,] PluNameList()
        {
#if ORDER
            String[,] names = new String[6, 20];
#else
            String[,] names = new String[6, 17];
#endif

            for (int i = 0; i < names.GetLength(0); i++)
            {
                for (int j = 0; j <= names.GetUpperBound(1); j++)
                    names[i, j] = GetPluName((i * NUMBER_OF_PLU_ON_PAGE) + j);
            }

            return names;
        }

        private static string GetPluName(int pluIndex)
        {
            List<IProduct> productList = new List<IProduct>();
            int label = pluIndex;
            string text = "PLU " + pluIndex;
            decimal price = 0m;

            try
            {
                if (Connector.Instance().CurrentSettings.GetProgramOption(Setting.DefineBarcodeLabelKeys) == PosConfiguration.ON)
                    productList.AddRange(Connector.Instance().SearchProductByBarcode(GetLabel(label)));
                else
                    productList.AddRange(Connector.Instance().SearchProductByLabel(GetLabel(label)));

                if (productList.Count == 0)
                {
                    text = PosMessage.NO_PRODUCT;
                }
                else
                {
                    text = productList[0].Name.Trim();
                    price = productList[0].UnitPrice;

                    if (text.Length > MAX_CHAR_IN_BUTTON && text.IndexOf(' ') > 0)
                    {
                        String[] splits = text.Split(new char[] { ' ' });
                        text = "";
                        int len = 0;
                        foreach (String part in splits)
                        {
                            if (part.Trim().Length == 0) continue;

                            if ((len > 0) && (len < MAX_CHAR_IN_BUTTON) && (len + part.Length) > MAX_CHAR_IN_BUTTON)
                            {
                                text = text + "\r" + part;
                                len = MAX_CHAR_IN_BUTTON + 1;
                                continue;
                            }
                            text = text + ' ' + part;
                            len += part.Length;
                        }
                    }
                }
            }
            catch
            {
                //Ignore exception
            }

            return String.Format("{0}\n{1:C}", text, new Number(price));
        }

        private static string[] GetLabel(int label)
        {
            if (!Connector.Instance().CurrentSettings.LabelMatrix.ContainsKey(label))
                throw new EntryException(PosMessage.UNDEFINED_LABEL);
            return Connector.Instance().CurrentSettings.LabelMatrix[label].Split(',');
        }

        private static List<Skin> GetQwertySkinList()
        {
            if (qwertySkinList != null)
                return qwertySkinList;

            qwertySkinList = new List<Skin>();
            Skin skin = null;
            int row = 0;
            int col = 0;

            qwertySkinList.Add(new Skin("btnDelete", PosMessage.DELETE, new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD1", "1", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD2", "2", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD3", "3", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD4", "4", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD5", "5", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD6", "6", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD7", "7", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD8", "8", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD9", "9", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD0", "0", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            skin = new Skin("btnBackspace", "←", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd);
            skin.colSpan=2;
            qwertySkinList.Add(skin);
            col = 0;
            row++;

            qwertySkinList.Add(new Skin("btnQ", "Q", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnW", "W", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnE", "E", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnR", "R", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnT", "T", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnY", "Y", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnU", "U", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnI", "I", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnO", "O", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnP", "P", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnĞ", "Ğ", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnÜ", "Ü", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnMinus", "-", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            col = 0;
            row++;

            qwertySkinList.Add(new Skin("btnA", "A", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnS", "S", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnD", "D", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnF", "F", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnG", "G", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnH", "H", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnJ", "J", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnK", "K", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnL", "L", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnŞ", "Ş", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnİ", "İ", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnUnderScore", "_", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnBackslash", "\\", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            col = 0;
            row++;

            qwertySkinList.Add(new Skin("btnZ", "Z", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnX", "X", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnC", "C", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnV", "V", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnB", "B", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnN", "N", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnM", "M", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnÖ", "Ö", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnÇ", "Ç", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnColon", ":", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnSemiColon", ";", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            skin = new Skin("btnEnter", "\n<─┘", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd);
            skin.colSpan = 2;
            skin.rowSpan = 2;
            qwertySkinList.Add(skin);
            col = 0;
            row++;

            qwertySkinList.Add(new Skin("btnSlash", "/", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnDot", ".", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnComma", ",", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            skin = new Skin("btnSpace", "\n└──┘", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd);
            skin.colSpan = 6;
            qwertySkinList.Add(skin);
            qwertySkinList.Add(new Skin("btnUpArrow", "↑", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));
            qwertySkinList.Add(new Skin("btnDownArrow", "↓", new Point(col++, row), KeyboardFont, Colors.KeyboardStart, Colors.KeyboardEnd));

            return qwertySkinList;
        }


        internal static List<Skin> GetSkin(WindowType windowsType)
        {
            List<Skin> skins = null;
            switch (windowsType)
            {
                case WindowType.Function:
                    skins = GetFunctionSkins();
                    break;
                case WindowType.Program:
                    skins = GetProgramSkinList();
                    break;
                case WindowType.PluList:
                    skins = GetPLUSkinList();
                    break;
                case WindowType.PluPage:
                    skins = GetPLUPageSkinList();
                    break;
                case WindowType.Payment:
                    skins = GetPaymentSkinList();
                    break;
                case WindowType.Input:
                    skins = GetQwertySkinList();
                    break;
            }

            return skins;
        }

        internal static void UpdateProducts()
        {
            pluSkinList = null;
        }
    }

    public struct Colors
    {
        //Button colors (end end start colors required for gradient look)
        public static Color CalculatorStart = Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(109)))), ((int)(((byte)(90)))));
        public static Color CalculatorEnd = Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(213)))), ((int)(((byte)(240)))));
        public static Color CalculatorText = Color.Black;
        public static Color PLUStart = Color.FromArgb(((int)(((byte)(154)))), ((int)(((byte)(188)))), ((int)(((byte)(159)))));
        public static Color PLUEnd = Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(213)))), ((int)(((byte)(194)))));
        public static Color PLUText = Color.Black;
        public static Color PageStart = Color.FromArgb(((int)(((byte)(179)))), ((int)(((byte)(163)))), ((int)(((byte)(199)))));
        public static Color PageEnd = Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(224)))), ((int)(((byte)(237)))));
        public static Color PageText = Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(73)))), ((int)(((byte)(122)))));
        public static Color NumericStart = Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
        public static Color NumericEnd = Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
        public static Color NumericText = Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(236)))), ((int)(((byte)(225)))));
        public static Color KeyboardStart = Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(236)))), ((int)(((byte)(225)))));
        public static Color KeyboardEnd = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(189)))), ((int)(((byte)(151)))));
        public static Color KeyboardText = Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(27)))), ((int)(((byte)(26)))));
        public static Color MenuStart = Color.FromArgb(((int)(((byte)(228)))), ((int)(((byte)(109)))), ((int)(((byte)(10)))));
        public static Color MenuEnd = Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(213)))), ((int)(((byte)(180)))));
        public static Color MenuText = Color.Black;
        public static Color CommandStart = Color.FromArgb(((int)(((byte)(197)))), ((int)(((byte)(155)))), ((int)(((byte)(5)))));
        public static Color CommandEnd = Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(226)))), ((int)(((byte)(23)))));
        public static Color CommandText = Color.Black;
        public static Color PaymentStart = Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(180)))), ((int)(((byte)(216)))));
        public static Color PaymentEnd = Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(230)))), ((int)(((byte)(242)))));
        public static Color PaymentText = Color.Black;
        public static Color ExecuteStart = Color.FromArgb(((int)(((byte)(146)))), ((int)(((byte)(208)))), ((int)(((byte)(80)))));
        public static Color ExecuteEnd = Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(213)))), ((int)(((byte)(194)))));
        public static Color ExecuteText = Color.Black;
        public static Color PriceStart = Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(139)))), ((int)(((byte)(84)))));
        public static Color PriceEnd = Color.FromArgb(((int)(((byte)(196)))), ((int)(((byte)(189)))), ((int)(((byte)(150)))));
        public static Color PriceText = Color.Black;
        public static Color AdjustmentStart = Color.FromArgb(((int)(((byte)(151)))), ((int)(((byte)(72)))), ((int)(((byte)(7)))));
        public static Color AdjustmentEnd = Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(193)))), ((int)(((byte)(146)))));
        public static Color AdjustmentText = Color.Black;
        public static Color ConfirmStart = Color.FromArgb(((int)(((byte)(56)))), ((int)(((byte)(111)))), ((int)(((byte)(46)))));
        public static Color ConfirmEnd = Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(144)))), ((int)(((byte)(62)))));
        public static Color ConfirmText = Color.Black;
        public static Color VoidStart = Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
        public static Color VoidEnd = Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(133)))), ((int)(((byte)(133)))));
        public static Color VoidText = Color.Black;

        public static Color DEF_START_COLOR = Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(242)))), ((int)(((byte)(242)))));
        public static Color DEF_END_COLOR = Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(213)))), ((int)(((byte)(240)))));

        public static Color InOrder_PriceLookUpStart = Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(135)))), ((int)(((byte)(227)))));
        public static Color InOrder_PriceLookUpEnd = Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
        public static Color InOrder_DrawerStart = Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(153)))), ((int)(((byte)(255)))));
        public static Color InOrder_DrawerEnd = Color.FromArgb(((int)(((byte)(229)))), ((int)(((byte)(204)))), ((int)(((byte)(255)))));
        
    }

}
