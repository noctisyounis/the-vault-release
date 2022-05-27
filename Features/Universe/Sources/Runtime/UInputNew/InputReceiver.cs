using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using static UnityEngine.InputSystem.InputAction;
using static UnityEngine.InputSystem.InputActionPhase;

namespace Universe
{
    public class InputReceiver : UBehaviour
    {
        /* Le Commentaire pour CAROO
         * 
         * + Une prefab original de TaskManager avec les inputs en enfants link a rien
         * + une prefab variant du task manager avec le binding du gameplay dit "classique" gun/katana
         * - une prefab variant du task manager avec le binding des UI
         * - on ajoutera au besoin une prefab de binding pour les phases exotiques
         * - ��������� voila
         * 
         * TODO POUR CHERIF :
         * - cr�er bouton "+ Menu" pour cr�er un menu task avec le prefab UI
        */

        #region Public

        public UnityEvent<CallbackContext> OnKey;
        public UnityEvent<CallbackContext> OnKeyDown;
        public UnityEvent<CallbackContext> OnKeyUp;

        #endregion


        #region Callbacks

        public void ReceiveOnKeyEvent( CallbackContext context )
        {
            DebugCallbackContext( "ReceiveOnKeyEvent", context );

            if( context.phase == Started )
                OnKeyDown.Invoke( context );
            else if( context.phase == Canceled )
                OnKeyUp.Invoke( context );
            else
                OnKey.Invoke( context );
        }

        #endregion

        private void DebugCallbackContext(string function, CallbackContext context)
        {
            Verbose( $"[{name}] {function} context = {context}, interaction = {context.interaction} context.control = {context.control}", this);
        }
    }
}