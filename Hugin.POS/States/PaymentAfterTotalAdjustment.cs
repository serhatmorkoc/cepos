using System;
using cr = Hugin.POS.CashRegister;
using Hugin.POS.Common;

namespace Hugin.POS.States
{
    class PaymentAfterTotalAdjustment : Payment
    {
        private static IState state = new PaymentAfterTotalAdjustment();
        private static Adjustment adjustment;
        private String adjustmentType = PosMessage.DISCOUNT_ALLOWED;
        public override Error NotImplemented { 
            get {
                if (adjustment.Method == AdjustmentType.Fee || adjustment.Method == AdjustmentType.PercentFee)
                    adjustmentType = PosMessage.COUNT_ALLOWED;              

                finalizeSaleErrorStr = String.Format("{0}\n{1}", adjustmentType,
                                                                 PosMessage.PROMPT_FINALIZE_SALE);
                return new Error(new Exception(finalizeSaleErrorStr), 
                                 new StateInstance(Payment.Instance),
                                 new StateInstance(Payment.Instance));

            }   
        }

        public static IState Instance(Adjustment adj)
        {
            adjustment = adj;
            return Instance(String.Empty);
        }
                    
        public static new IState Instance(String message)
        {
            input = new Number();
            if (!message.Equals(String.Empty))
                DisplayAdapter.Cashier.Show(message);
            return state;
        }

        public override void Void()
        {
            //Correction();
            DisplayAdapter.Cashier.Show(PosMessage.CANNOT_VOID_ITEM);
            System.Threading.Thread.Sleep(1500);
            cr.State = state;
        }

        public override void Correction()
        {
            try
            {
                cr.Document.UndoAdjustment(true);
                cr.State = SellingAfterSubtotalCorrection.Instance();
            }
            catch (CmdSequenceException)
            {
                cr.State = AlertCashier.Instance(new Error(new NoAdjustmentException()));
            }
            catch (NoAdjustmentException ae)
            {
                AlertCashier.Instance(new Error(ae));
            }

        }
#if ORDER
        public override void SendOrder()
        {
            cr.State = States.ConfirmVoid.Instance(new Confirm(String.Format("{0}\t{1}\n{2}", PosMessage.ORDER_TR, cr.Document.TotalAmount, PosMessage.CONFIRM_SEND_ORDER), new StateInstance(CloseOrder)));
        }

        public static IState CloseOrder()
        {
            try
            {
                cr.Document.CloseOrder();
            }
            catch (OrderServerNoMatcedDevice osnmd)
            {
                return States.AlertCashier.Instance(new Confirm(PosMessage.NO_MATCHED_EFT_POS, States.Start.Instance, States.Start.Instance));
            }
            catch (AnyConnectedEftPosException acepe)
            {
                return States.AlertCashier.Instance(new Confirm(PosMessage.ANY_CONNECTED_EFT_POS, States.Start.Instance, States.Start.Instance));
            }
            catch (TimeoutException toe)
            {
                return States.ConfirmCashier.Instance(new Confirm(PosMessage.TIMEOUT_EX_SEND_AGAIN, new StateInstance(CloseOrder), States.Start.Instance));
            }

            if (cr.Document.Status == DocumentStatus.Voided)
            {
                cr.Document.Void();
                return States.AlertCashier.Instance(new Confirm(PosMessage.DOCUMENT_VOIDED_BY_EFT_POS, States.Start.Instance, States.Start.Instance));
            }

            return States.AlertCashier.Instance(new Confirm(PosMessage.PAID_IS_DONE_TY, States.Start.Instance, States.Start.Instance));
        }
#endif
    }
}
