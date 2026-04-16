import time
import sys
import os
import pyperclip
from pywinauto import Desktop
from pywinauto.keyboard import send_keys

# VITPredittore v2.2 - SUPER AUTO INSTALLER (V2 - ULTRA COMPATIBILE)

def main():
    print("====================================================")
    print("   VITPredittore v2.2 - SUPER AUTO INSTALLER V2    ")
    print("====================================================")
    
    bot_path = r"d:\soldi\VITPredittore_Sacro.cs"
    if not os.path.exists(bot_path):
        print(f"ERRORE: Il file {bot_path} non esiste!")
        return

    with open(bot_path, 'r', encoding='utf-8') as f:
        sacred_code = f.read()

    print("\n[1/5] Ricerca avanzata finestra cTrader...")
    try:
        des = Desktop(backend="uia")
        windows = des.windows()
        
        target_window = None
        for win in windows:
            title = win.window_text()
            if "ctrader" in title.lower():
                target_window = win
                break
        
        if target_window is None:
            print("ERRORE: Non trovo nessuna finestra con 'cTrader' nel titolo.")
            print("Finestre trovate (controlla se c'è quella del tuo broker):")
            for win in windows[:10]: print(f" - {win.window_text()}")
            return
            
        print(f"   Trovato: {target_window.window_text()}")
        target_window.set_focus()
        time.sleep(1)

        # [2/5] Navigazione Algo
        print("[2/5] Accesso sezione Algo...")
        # Scorciatoia universale per Algo in quasi tutte le versioni
        send_keys("^%a") 
        time.sleep(2)

        # [3/5] Nuovo cBot
        print("[3/5] Creazione nuovo cBot...")
        # Proviamo prima il tasto fisico, poi la tastiera
        new_btn = target_window.child_window(title_re=".*New.*", control_type="Button")
        if new_btn.exists(timeout=2):
            new_btn.click_input()
        else:
            send_keys("^n")
        time.sleep(2)

        # [4/5] Incolla Codice
        print("[4/5] Iniezione Codice Sacro...")
        # Click al centro per sicurezza
        rect = target_window.rectangle()
        center_x = rect.left + (rect.width() // 2)
        center_y = rect.top + (rect.height() // 2)
        import pyautogui
        pyautogui.click(center_x, center_y)
        
        time.sleep(0.5)
        send_keys("^a{BACKSPACE}")
        time.sleep(1)
        pyperclip.copy(sacred_code)
        send_keys("^v")
        time.sleep(2)

        # [5/5] Build
        print("[5/5] Compilazione finale...")
        build_btn = target_window.child_window(title_re=".*Build.*", control_type="Button")
        if build_btn.exists(timeout=2):
            build_btn.click_input()
        else:
            send_keys("^b") # Standard cTrader build shortcut
            
        print("\n====================================================")
        print("   MISSIONE COMPIUTA! Il Bot è caricato e compilato.")
        print("   Vai su cTrader e premi PLAY su EURUSD M5.")
        print("====================================================")

    except Exception as e:
        print(f"\nERRORE: {e}")

if __name__ == "__main__":
    main()
