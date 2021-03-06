# Scribble

Scribble is a word game in which two human opponents (player1 vs player2) score points by placing letters onto a game-board which is divided into a 15×15 grid of cells. The letters must form words which, in crossword fashion, flow left to right in rows or downwards in columns consecutively.

### General Information

The game is played by two opponents on a board with a 15×15 grid of cells, each of which accommodates a single letter. In their turns, players can try to form an acceptable word or they can exchange some of their letters or they can pass their turns. A word is acceptable only if it is located in given dictionary.

### Rules

    1. Initially the dictionary words and letter reservoir file must be loaded. Dictionary contains the list of acceptable words, letter reservoir file contains the amount and point of each letter.  

    2. The game begins with an empty board. Each player is given a “bag”, containing 7 random letters taken from reservoir. 

    3. In their turns, one of the following actions is possible for each player; 
        a. Players must form a word by using their letters (and also the letters on the board). Once placed on the board, every accepted word stays fixed until the end of the game. 
        b. Players can return any amount of their letters to letter reservoir and get the same amount of the new letters randomly.
        c. Players can pass their turns. 

    4. After placing an acceptable word, this word and any other newly formed words' scores are calculated by summing the individual points of all included letters for each new word. This score is added to player’s total score. Automatically, player's bag refilled, getting new random letters from reservoir up to 7 letters.

    5. At least one empty cell must be left (or a border of the board) at the beginning and end of the last formed word. 

    6. Arrow keys must be used for selecting cells before letter placement. 

    7. Players can query the dictionary (max. 3 times for each turn) by using letters and dots. A dot represents any single letter.

    8. The game continues until two opponents pass their turns twice (4 consecutive passes) or reservoir gets empty.

### Screenshots

![Screenshot](https://github.com/ZagorTenay/Scribble/blob/master/ss1.png)

![Screenshot](https://github.com/ZagorTenay/Scribble/blob/master/ss2.png)

![Screenshot](https://github.com/ZagorTenay/Scribble/blob/master/ss3.png)
