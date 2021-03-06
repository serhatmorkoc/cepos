using System;
using System.Collections;
using cr = Hugin.POS.CashRegister;
using Hugin.POS.Common;

namespace Hugin.POS.States
{
    //Any class that overrides Start should be included in 
    //ServerConnection.BackgroundWorker check
    class Start : State
    {
        private static IState state = new Start();
        private const String cashierMsg = "{0}\t{1:dd/MM/yy}\n{2}\t{1:HH:mm}";

        public override bool IsIdle
        {
            get
            {
                return true;
            }
        }

        public static IState Instance()
        {
            if (cr.Printer == null) Chassis.RestartProgram(false);
            if (cr.CurrentCashier == null || cr.CurrentManager == null)
                return Login.Instance();
            if (cr.Document.Status == DocumentStatus.Paying)
                return Payment.Continue();
            if (cr.Document.State == DocumentPaying.Instance())
                return Payment.Continue();
            if (cr.Document.Adjustments.Length != 0)
            {
                decimal total = 0;
                for (int adj = 0; adj < cr.Document.Adjustments.Length; adj++)
                {
                    total += cr.Document.Adjustments[adj].NetAmount;
                }
                if (total != 0)
                    return Payment.Continue();
            }
            cr.Document.State = DocumentOpen.Instance();
            //Turn off satis var led
            if (cr.Document.IsEmpty)
                DisplayAdapter.Cashier.LedOff(Leds.Sale);
            //Turn off customer led
            if (cr.Document.Customer == null)
                DisplayAdapter.Cashier.LedOff(Leds.Customer);
        	//Show main menu to cashier and customer
            
            //check name if length bigger-smaller than 14
            String name = cr.CurrentCashier.Name;
            if(name.Length > 14)
            {
                name = name.Substring(0, 14);
            }

            System.Threading.Thread.Sleep(100);
            DisplayAdapter.Cashier.Show(cashierMsg, cr.Document.Name, DateTime.Now, name);
            DisplayAdapter.Customer.Show(PosMessage.WELCOME);
            cr.Printer.ClearDisplay();

            if (cr.Item != null)
                cr.Item.Reset();
            if (cr.Item is VoidItem)
                cr.Item = new SalesItem();
            Payment.ResetPayment();
#if ORDER
            if (!cr.Document.IsEmpty && cr.Document.ReturnReason == "WAITING_FOR_PAYMENT")
            {
                /*DisplayAdapter.Cashier.Show("ÖDEME BEKLENİYOR..");
                System.Threading.Thread.Sleep(2000);
                return Selling.CloseOrder();*/

                // Çıkış için document iptal edilebilir, çıkış fonk VoidLastOrder eklenebilir
                cr.Document.ReturnReason = null;
                return States.AlertCashier.Instance(new Confirm(PosMessage.WAITING_PAYMENT, new StateInstance(Selling.CloseOrder)));
            }
            else if (!cr.Document.IsEmpty)
            {
                return Selling.Instance();
            }
            else
            {
                return state;
            }
#else
            return cr.Document.IsEmpty ? state : Selling.Instance();
#endif
            
        }

        public static IState SilentInstance() { 
            IState state = null;
            try
            {
                DisplayAdapter.Cashier.Pause();
                DisplayAdapter.Customer.Pause();
                state = Instance();
            }
            finally {
                DisplayAdapter.Cashier.Play();
                DisplayAdapter.Customer.Play();
            }

            return state;
        }

        public static IState Instance(String message)
        {
            if (cr.Document.Status == DocumentStatus.Paying)
                return Payment.Continue();
            if (!message.Equals(String.Empty))
                DisplayAdapter.Cashier.Show(message);
            //Turn off satis var led
            DisplayAdapter.Cashier.LedOff(Leds.Sale);
            //Turn off customer led
            if (cr.Document.Customer == null)
                DisplayAdapter.Cashier.LedOff(Leds.Customer);
            return cr.Document.IsEmpty ? state : Selling.Instance();
        }
        public override void Escape()
        {
        	cr.State = ConfirmLogout.Instance();
        }

        public override void Repeat()
        {
            DocumentFileHelper[] orders = DocumentFileHelper.GetOpenOrders();
            
            if (orders.Length > 0)
            {
                SalesDocument doc = orders[0].LoadDocument();
                String cashiername = doc.SalesPerson.Name;
                cashiername = cashiername.Substring(0, Math.Min(14, cashiername.Length));
                String label = doc.Name.Substring(0, 3) + " NO:{0:D4}\t{1:dd/MM/yy}\n{2}\t{1:HH:mm}";
                label = String.Format(label, doc.Id, doc.CreatedDate, cashiername);
                CommandMenu.RepeatDocument(new MenuLabel(label, orders[0]));
            }
            BackgroundWorker.ProcessTcpOrder();

        }
        public override void Enter()
        {
        }
        public override void Numeric(char c)
        {
            cr.State = States.EnterNumber.Instance();
            cr.State.Numeric(c);
        }
        public override void Seperator()
        {
            cr.State = States.EnterNumber.Instance();
            cr.State.Seperator();
        }

        public override void Alpha(char c)
        {
            int i = 0;
            if (Parser.TryInt(c.ToString(), out i))
                Numeric(c);
            else base.Alpha(c);
        }

        public override void Customer()
        {
            CustomerInfo.ReturnCancel = null;

            if (cr.Document.Customer != null)
            {
                String msg = String.Format("{0}\n{1}", cr.Document.Customer.Name, PosMessage.CONFIRM_VOID_CURRENT_CUSTOMER);
                cr.State = ConfirmCashier.Instance(new Confirm(msg, CustomerInfo.ConfirmVoidCustomer));
            }
            else cr.State = States.CustomerInfo.Instance();

        }

        public override void SalesPerson()
        {

            //Check whether cashier has authorization to assign salesPerson


            if (cr.Document.SalesPerson == null)
                cr.State = EnterClerkNumber.Instance(PosMessage.CLERK_ID,
                                            new StateInstance<ICashier>(cr.Document.ConfirmSalesPerson),
                                            new StateInstance(Start.Instance));
            else
            {
                String prompt = String.Format("{0}\n{1}", cr.Document.SalesPerson.Name.TrimEnd(), PosMessage.VOID_SALESPERSON);
                Confirm confirmVoidSalesPerson = new Confirm(prompt,
                                                         new StateInstance(cr.Document.VoidSalesPerson),
                                                         new StateInstance(Start.Instance));
                cr.State = ConfirmCashier.Instance(confirmVoidSalesPerson);
            }
        }

        public override void PriceLookup()
        {
            cr.State = EnterString.Instance(PosMessage.PRICE_LOOKUP, new StateInstance<String>(cr.PriceLookup));
        }

        public override void Price()
        {
            if (cr.IsAuthorisedFor(Authorizations.LabelAmountChanging))
                cr.State = EnterUnitPrice.Instance();
            else
                throw new CashierAutorizeException();
        }

        public override void TotalAmount()
        {
            cr.State = States.EnterTotalAmount.Instance();
        }

        public override void LabelKey(int labelKey)
        {
            System.Collections.Generic.List<IProduct> sList = new System.Collections.Generic.List<IProduct>();
            if (cr.DataConnector.CurrentSettings.GetProgramOption(Setting.DefineBarcodeLabelKeys) == PosConfiguration.ON)
                sList = cr.DataConnector.SearchProductByBarcode(Label.GetLabel(labelKey));
            else
                sList = cr.DataConnector.SearchProductByLabel(Label.GetLabel(labelKey));

            if (sList.Count == 0)
            {
                cr.State = AlertCashier.Instance(new Error(new ProductNotFoundException()));
                return;
            }

            // Check Printer Status before add label
            cr.Printer.CheckPrinterStatus();

            MenuList itemList = new MenuList();
            foreach (IProduct p in sList) {
                if (cr.Item is VoidItem)
                    cr.Item = new SalesItem();
                SalesItem si = (SalesItem)cr.Item.Clone();
                si.Product = p;
                if (p.Status == ProductStatus.Weighable && cr.Scale != null)
                {
                    try
                    {
                        si.Quantity = cr.Scale.GetWeight(p.UnitPrice);
                    }
                    catch (Exception) { }
                }
                itemList.Add(si);
            }

            if (itemList.Count == 1)
            {
                if (itemList.MoveNext())
                    cr.Execute(((IDoubleEnumerator)itemList).Current);              
            }
            else
            {
                cr.State = ListLabel.Instance(itemList,
                                               new ProcessSelectedItem<IProduct>(cr.Execute),
                                               labelKey);
            }
        }    
    
		public override void Program()
		{
			cr.State = States.SetupMenu.Instance();
            DisplayAdapter.Customer.Show(PosMessage.WELCOME);
		}        
		public override void Document()
		{
			MenuList docTypes = new MenuList();

            if (!(cr.Document is Receipt) && cr.Document.Id>0)
            {
                cr.State = States.ConfirmCashier.Instance(new Confirm(PosMessage.CONFIRM_VOID_DOCUMENT, ConfirmVoidDocument, Instance));
                return;
            }

            int standing = 1;
            
            AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.INVOICE, new Invoice());           
            AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.E_INVOICE, new EInvoice());
            AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.E_ARCHIVE, new EArchive());
            AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.MEAL_TICKET, new MealTicket());
            AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.CAR_PARKIMG, new CarParkDocument());
            AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.RECEIPT_TR, new Receipt());

            if(cr.IsAuthorisedFor(Authorizations.AdvanceAndReturnDocAuth)) // Check cashier auth for advance
                AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.ADVANCE, new Advance());

            AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.COLLECTION_INVOICE, new CollectionInvoice());
            AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.CURRENT_ACCOUNT_COLLECTION, new CurrentAccountDocument());
            AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.SELF_EMPLOYEMENT_INVOICE, new SelfEmployementInvoice());

            /*  İade Fişi */
            if (cr.IsAuthorisedFor(Authorizations.AdvanceAndReturnDocAuth))
                AddMenuLabel(docTypes, PosMessage.SELECT_DOCUMENT + "\t" + standing++ + "\n" + PosMessage.RETURN_DOCUMENT_TR, new ReturnDocument());


            cr.State = ListDocument.Instance(docTypes, new ProcessSelectedItem<SalesDocument>(cr.ChangeDocumentType));
            DisplayAdapter.Customer.Show(PosMessage.WELCOME);
        }

        //:to do: function is called by also other classes, so it is static but it may not be here
        public static void AddMenuLabel(MenuList docTypes, String message, SalesDocument document)
        {
            if (cr.Printer.CanPrint(document))
                docTypes.Add(new MenuLabel(message, document));
        }

        public static IState ConfirmVoidDocument()
        {
            cr.Document.Void();
            return Instance();
        }

        public override void Command()
        {
            MenuList commandMenu = new MenuList();
            commandMenu.Clear();
            int index = 1;
            if (cr.IsDesktopWindows)
            {
                commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.REPEAT_DOCUMENT));
            }

            commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.RESUME_DOCUMENT));
            commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.CUSTOMER_ENTRY));

            commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.EFT_POS_OPERATIONS));

            if (cr.IsDesktopWindows)
            {
                //commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.FAST_PAYMENT));
                commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.TABLE_MANAGEMENT));
                commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.ENTER_CASH));
                commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.RECEIVE_CASH));
                //commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.RECEIVE_CHECK));
                //commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.RECEIVE_CREDIT));
                commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.COMMAND_CALCULATOR));
                commandMenu.Add(CommandMenu.AddLabel(index++, PosMessage.TALLYING));
            }
            cr.State = CommandMenu.Instance(commandMenu);
            DisplayAdapter.Customer.Show(PosMessage.WELCOME);
        }

        public override void CardPrefix()
        {
            cr.State = EnterCardNumber.Instance(PosMessage.ENTER_CADR_CODE,
                                                new StateInstance<string>(CustomerInfo.SetCurrentCustomer),
                                                new StateInstance(CustomerInfo.Instance));                       
        }
        public override void BarcodePrefix()
        {
            cr.State = EnterString.Instance(PosMessage.ENTER_BARCODE,
                                                new StateInstance<string>(BarcodeMenu.Instance),
                                                new StateInstance(BarcodeMenu.Instance));
        }
        public override void Report()
        {
            cr.State = States.ReportMenu.Instance();
        }
        public override void CashDrawer()
        {
            cr.Printer.OpenDrawer();
        }

        public override void UpArrow()
        {
            cr.Printer.Feed();
        }
        public override void OnEntry()
        {
            base.OnEntry();
        }

        void SecurityConnector_CashierCaptured(object sender, EventArgs e)
        {
            ICashier cashier = sender as ICashier;
            if (cashier == cr.CurrentCashier)
                return;
            DisplayAdapter.Cashier.Show(String.Format("İKİNCİ KASİYER\n{0}", cashier.Name));
            Instance();
        }

        private static IState VoidLastOrder()
        {
            cr.Document.Void();
            cr.Document = new Receipt();
            return States.Start.Instance();
        }
    }
}
