# Tugas Besar 1 Strategi Algoritma:<br>Robocode (Voratank)

## Deskripsi
Robocode adalah permainan pemrograman yang bertujuan untuk membuat kode bot dalam bentuk tank virtual untuk berkompetisi melawan bot lain di arena. Pertempuran Robocode berlangsung hingga bot-bot bertarung hanya tersisa satu seperti permainan Battle Royale, karena itulah permainan ini dinamakan Tank Royale. Nama Robocode adalah singkatan dari "Robot code," yang berasal dari [versi asli/pertama permainan ini](https://robocode.sourceforge.io/). Robocode Tank Royale adalah evolusi/versi berikutnya dari permainan ini, di mana bot dapat berpartisipasi melalui Internet/jaringan.  
Dalam permainan ini, pemain berperan sebagai programmer bot dan tidak memiliki kendali langsung atas permainan. Pemain hanya bertugas untuk membuat program yang menentukan logika atau "otak" bot. Program yang dibuat akan berisi instruksi tentang cara bot bergerak, mendeteksi bot lawan, menembakkan senjatanya, serta bagaimana bot bereaksi terhadap berbagai kejadian selama pertempuran.

## Algoritma *Greedy*
| Strategi | Heuristic | Alasan |
| ----- | ----- | ----- |
| Spam Fire Damage (Voratank) | Menembak bot lawan yang terdeteksi oleh radar secara terus menerus dengan damage yang besar. | Agar bot lawan cepat kehabisan energi ketika ditembak oleh bot ini. |
| Pergerakan zig zag (Vorabot) | Belok kiri dan belok kanan dengan sudut yang sama. | Agar sulit ditembak oleh bot musuh. |
| Kunci target (UnpredictaBot) | Fungsi Rescan() saat *bearing* \= 0\. | Agar bot dapat fokus terhadap satu bot musuh dan mengurangi serangan yang meleset. |
| Spam Ram Fire (SmartVoraRam) | Bot berjalan ke bot musuh dan terus menabraknya sambil menembak. | Agar dapat menghabiskan energi bot musuh jauh lebih cepat. |

## Requirement
- Java
- Net 9.0

## Cara menjalankan program
1. Download `robocode-tankroyale-gui-0.30.0.jar` dari [*repository* GitHub ini](https://github.com/Ariel-HS/tubes1-if2211-starter-pack/releases).
2. Jalankan file .jar tersebut
    ```
    java -jar robocode-tankroyale-gui-0.30.0.jar
    ```
    Atau jika ingin *build* sendiri, ikuti langkah 3-5:
    
3. Masuk ke *directory* "tank-royale-0.30.0"
    ```
    cd tank-royale-0.30.0
    ```
4. (Clean &) Build gui-app
    ```
    ./gradlew :gui-app:clean
    ./gradlew :gui-app:build
    ```
5. Jalankan jar
    ```
    java -jar ./gui-app/build/libs/robocode-tankroyale-gui-0.30.0.jar
    ```

## Cara menggunakan program
1. Pada program, lakukan Ctrl+D atau klik Config > Bot Root Directories
2. Klik Add > pilih folder yang berisi bot > Open > Ok
3. Lakukan Ctrl+B atau klik Battle > Start Battle
4. Pilih bot pada kotak kiri atas ("Bot Directories (local only)") > Boot →
    * Bot akan masuk ke kotak kanan atas ("Booted Bots (local only)"). Jika berhasil di-*boot*, akan muncul pada kotak kiri bawah ("Joined Bots (local/remote)")
    * Pilih bot dan Klik ← Unboot untuk men-*unboot* bot yang terpilih.
    * Klik ← Unboot All untuk men-*unboot* semua bot pada kotak kanan atas.
5. Pilih bot pada kotak kiri bawah > Add →
    * Bot akan masuk ke kotak kanan bawah ("Selected Bots (battle participants)").
    * Klik Add All → untuk menambahkan semua bot dari kotak kiri bawah.
    * Pilih bot dan Klik ← Remove untuk menghapus bot dari kotak kanan bawah.
    * Klik ← Remove All untuk menghapus semua bot dari kotak kanan bawah.
6. Klik Start Battle

## Tips
* Klik Battle > Setup Rules untuk mengubah konfigurasi permainan
    * Mode permainan
    * Ukuran arena
    * Jumlah ronde
    * *Turn count* maksimum
    * Dan sebagainya

## Author
- 12821046 - Fardhan Indrayesa
- 13523132 - Jonathan Levi
- 13622076 - Ziyan Agil Nur Ramadhan
