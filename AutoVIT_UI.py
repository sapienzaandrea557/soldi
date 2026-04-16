import pyautogui
import pyperclip
import time
import os
import sys

# VITPredittore v2.2 - AUTOMAZIONE UI (PYTHON)
# Questo script automatizza il caricamento del bot "Sacro" in cTrader cliccando e scrivendo per te.

def get_sacred_code(path):
    if os.path.exists(path):
        with open(path, 'r', encoding='utf-8') as f:
            return f.read()
    else:
        print(f"ERRORE: File {path} non trovato!")
        sys.exit()

def main():
    print("====================================================")
    print("   VITPredittore v2.2 - AUTOMAZIONE CTRADER (UI)   ")
    print("====================================================")
    print("\n[INFO] Leggo il Codice Sacro...")
    
    bot_path = r"d:\soldi\VITPredittore_Sacro.cs"
    code = get_sacred_code(bot_path)
    
    print("\n[IMPORTANTE] Per favore, apri cTrader e mettilo a tutto schermo.")
    input("Premi INVIO quando sei pronto...")

    # Passaggio 1: Registrazione posizioni (Dinamico per ogni schermo)
    print("\n--- CONFIGURAZIONE POSIZIONI (Premi INVIO con il mouse sopra il tasto indicato) ---")
    
    input("1. Sposta il mouse sopra l'icona 'ALGO' (quella con < > a sinistra) e premi INVIO...")
    pos_algo = pyautogui.position()
    print(f"   Posizione Algo salvata: {pos_algo}")

    input("2. Sposta il mouse sopra il tasto blu 'NUOVO' (New cBot) e premi INVIO...")
    pos_new = pyautogui.position()
    print(f"   Posizione Nuovo salvata: {pos_new}")

    input("3. Sposta il mouse sopra l'AREA DI TESTO (dove si scrive il codice) e premi INVIO...")
    pos_text = pyautogui.position()
    print(f"   Posizione Area Testo salvata: {pos_text}")

    input("4. Sposta il mouse sopra il tasto 'BUILD' (il Martello) e premi INVIO...")
    pos_build = pyautogui.position()
    print(f"   Posizione Build salvata: {pos_build}")

    print("\n--- INIZIO AUTOMAZIONE (Non toccare mouse e tastiera!) ---")
    time.sleep(2)

    # 1. Click su Algo
    pyautogui.click(pos_algo)
    time.sleep(1)

    # 2. Click su Nuovo
    pyautogui.click(pos_new)
    time.sleep(2)

    # 3. Seleziona tutto il vecchio codice e cancella
    pyautogui.click(pos_text)
    time.sleep(0.5)
    pyautogui.hotkey('ctrl', 'a')
    time.sleep(0.5)
    pyautogui.press('backspace')
    time.sleep(0.5)

    # 4. Incolla il nuovo codice
    print("[LOG] Incollo il Codice Sacro...")
    pyperclip.copy(code)
    pyautogui.hotkey('ctrl', 'v')
    time.sleep(2)

    # 5. Compila
    print("[LOG] Compilazione in corso...")
    pyautogui.click(pos_build)
    
    print("\n====================================================")
    print("   SUCCESSO! Il Bot Sacro è stato caricato e compilato.")
    print("   Ora devi solo premere PLAY sul grafico M5.")
    print("====================================================")

if __name__ == "__main__":
    try:
        import pyperclip
        import pyautogui
    except ImportError:
        print("[ERR] Mancano le librerie! Installa con: pip install pyautogui pyperclip")
        sys.exit()
    
    main()
