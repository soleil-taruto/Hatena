using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.CSSolutions
{
	public static class CSResources
	{
		#region 予約語リスト

		/// <summary>
		/// ここに含まれる単語は置き換えない。
		/// </summary>
		public static string 予約語リスト = @"

; ====
; C#の予約語
; ====

; --https://ufcpp.net/study/csharp/ap_reserved.html

; キーワード
abstract
as
async
await
base
bool
break
byte
case
catch
char
checked
class
const
continue
decimal
default
delegate
do
double
else
enum
event
explicit
extern
false
finally
fixed
float
for
foreach
goto
if
implicit
in
int
interface
internal
is
lock
long
namespace
new
null
object
operator
out
override
params
private
protected
public
readonly
ref
return
sbyte
sealed
short
sizeof
stackalloc
static
string
struct
switch
this
throw
true
try
typeof
uint
ulong
unchecked
unsafe
ushort
using
virtual
volatile
void
while

; 文脈キーワード
add
dynamic
get
partial
remove
set
value
var
where
yield
when


; ====
; 外部シンボル名
; 注意：これらをリネームしてもビルドは通る。実行時にエラーになる。
; ====

AddFontResourceEx
ClientToScreen
EnumWindows
GetWindowText
RemoveFontResourceEx


; ====
; 名前空間 / クラス名 / 型名 / メンバー名
; ====

; e20201109_YokoActTK

AccessControlType
Action
Add
AddAccessRule
AddrOfPinnedObject
AggregateException
AllDirectories
Alloc
Allow
Anchor
AppDomain
Append
Application
ArgumentException
Array
Assembly
AutoScaleDimensions
AutoScaleMode
AutoSize
Begin
BeginInvoke
CheckState
Checked
Clear
Click
ClientSize
Close
Collect
Color
Combine
Comparison
Compress
CompressionMode
ComputeHash
Console
Contains
ContainsKey
Controls
Copy
CopyTo
Cos
Count
Create
CreateDirectory
Current
CurrentDomain
DateTime
Decompress
Delete
Dequeue
Dictionary
Directory
Dispose
DllImport
DropDownStyle
DxLibDLL
EnableVisualStyles
Encoding
EndsWith
Enqueue
Enum
Enumerable
Environment
Equals
Error
EventArgs
Exception
ExceptionObject
Exists
Exit
File
FileAccess
FileMode
FileStream
FirstOrDefault
Flags
Font
Form
FormClosed
FormClosedEventArgs
FormClosing
FormClosingEventArgs
Format
FormattingEnabled
Free
FromArgb
FullControl
Func
GC
GCHandle
GCHandleType
GZipStream
GetByteCount
GetBytes
GetCommandLineArgs
GetCurrentProcess
GetDirectoryName
GetEncoding
GetEntryAssembly
GetEnumerator
GetEnvironmentVariable
GetFileName
GetFiles
GetFullPath
GetHashCode
GetObject
GetRange
GetString
GetValues
Guid
Handle
IDisposable
IEnumerable
IEnumerator
IEqualityComparer
Icon
Id
IndexOf
IntPtr
IsNaN
IsNullOrEmpty
Items
Join
Keys
LayoutKind
Length
LinkDemand
List
Load
Location
Main
Margin
Math
Max
MaxValue
MaximizeBox
MemoryStream
Message
MessageBox
MessageBoxButtons
MessageBoxIcon
MethodInvoker
Microsoft
Min
MinimizeBox
MinimumSize
MoveNext
Msg
Mutex
MutexAccessRule
MutexRights
MutexSecurity
Name
NewGuid
Now
OK
Open
PI
Parse
Path
PerformLayout
Pinned
Position
Predicate
Process
Queue
RNGCryptoServiceProvider
RandomNumberGenerator
Range
Read
ReadAllBytes
ReadAllLines
ReleaseMutex
RemoveAt
Replace
ResumeLayout
Run
SHA512
STAThread
SearchOption
SecurityAction
SecurityIdentifier
SecurityPermission
SecurityPermissionFlag
Seek
SeekOrigin
Select
SelectedIndex
Sequential
SessionEnding
SessionEndingEventArgs
SessionEndingEventHandler
SetCompatibleTextRenderingDefault
SetCurrentDirectory
Show
ShowInTaskbar
Shown
Sin
Size
SizeGripStyle
Sleep
Sort
Split
Sqrt
Start
StartPosition
StartsWith
Stream
StreamReader
StreamWriter
StringBuilder
StructLayout
Substring
SuspendLayout
SuspendLayout
SystemEvents
TabIndex
TabStop
Tan
Text
Thread
ThreadException
ThreadExceptionEventArgs
ThreadExceptionEventHandler
ToArray
ToInt64
ToList
ToLower
ToString
ToUpper
TopMost
Trim
UInt16
UInt64
UTF8
UnhandledException
UnhandledExceptionEventArgs
UnhandledExceptionEventHandler
UnmanagedCode
UseVisualStyleBackColor
Value
Visible
WParam
WaitOne
WellKnownSidType
Where
Win32
WndProc
WorldSid
Write
WriteAllBytes
WriteByte
WriteLine
Zero

; 追加 e20200928_NovelAdv

Cast
Concat
GetFileNameWithoutExtension
HashSet
Insert
Skip

; 追加 e20201003_NovelAdv

Any
First
GetNames
RemoveAll

; 追加 e20201010_TateShoot

Abs
Repeat

; 追加 e20201018_TateShoot

NotImplementedException

; 追加 e20201020_YokoShoot

ASCII
Convert

; 追加 e20201115_Dungeon -- discontinued

Distinct
IsMatch
Regex

; 追加 e20201210_YokoActTM

Key

; 追加 e20201211_Hakonoko

Take
MaxDropDownItems


; ====
; その他 / 特別扱い
; ====

; 定番の名前空間
Charlotte

; デザイナで自動生成されるメソッド
;InitializeComponent

";

		#endregion

		#region 予約語クラス名リスト

		/// <summary>
		/// ここに含まれる単語は置き換えない。
		/// (この単語).(後続の単語).(後続の単語).(後続の単語) ... の「後続の単語」についても置き換えない。
		/// </summary>
		public static string 予約語クラス名リスト = @"

DX
System

";

		#endregion

		#region ランダムな単語リスト

		public static string ランダムな単語リスト = @"

; ====
; 太陽系の天体
; ====

; ---- 恒星 ----

Sun

; ---- 惑星 ----

Mercury
Venus
Earth
Mars
Jupiter
Saturn
Uranus
Neptune

; ---- 準惑星 ----

; 地球と火星の間
Ceres

; 海王星の外側
Pluto
Haumea
Makemake
Eris

; ---- 衛星 ----

; -- Earth
; 1
Moon

; -- Mars
; 1
Phobos
; 2
Deimos

; -- Jupiter
; 1
Metis
; 2
Adrastea
; 3
Amalthea
; 4
Thebe
; 5
					;Io
; 6
Europa
; 7
Ganymede
; 8
Callisto
; 9
Themisto
; 10
Leda
; 11
Himalia
; 12
Ersa
; 13
Pandia
; 14
Elara
; 15
Lysithea
; 16
Dia
; 17
Carpo
; 18
;S/2003 J 12
; 19
Valetudo
; 20
Euporie
; 21
;S/2003 J 18
; 22
Harpalyke
; 23
Hermippe
; 24
;S/2017 J 7
; 25
Euanthe
; 26
Thyone
; 27
;S/2016 J 1
; 28
Mneme
; 29
;S/2017 J 3
; 30
Iocaste
; 31
Praxidike
; 32
Ananke
; 33
;S/2003 J 16
; 34
Thelxinoe
; 35
Orthosie
; 36
Helike
; 37
Eupheme
; 38
;S/2010 J 2
; 39
;S/2017 J 9
; 40
;S/2017 J 6
; 41
;S/2011 J 1
; 42
Kale
; 43
Chaldene
; 44
Taygete
; 45
Herse
; 46
Kallichore
; 47
Kalyke
; 48
;S/2003 J 19
; 49
Pasithee
; 50
;S/2003 J 10
; 51
;S/2003 J 23
; 52
Philophrosyne
; 53
Cyllene
; 54
;S/2010 J 1
; 55
Autonoe
; 56
Megaclite
; 57
Eurydome
; 58
;S/2017 J 5
; 59
;S/2017 J 8
; 60
Pasiphae
; 61
Callirrhoe
; 62
;S/2011 J 2
; 63
;S/2017 J 2
; 64
Isonoe
; 65
Aitne
; 66
Hegemone
; 67
Sponde
; 68
Eukelade
; 69
;S/2003 J 4
; 70
Erinome
; 71
Arche
; 72
Eirene
; 73
;S/2003 J 9
; 74
Carme
; 75
Aoede
; 76
Kore
; 77
Sinope
; 78
;S/2017 J 1
; 79
;S/2003 J 2

; -- Saturn
; 1
;S/2009 S 1
; 2
Pan
; 3
Daphnis
; 4
Atlas
; 5
Prometheus
; 6
Pandora
; 7a
Epimetheus
; 7b
Janus
; 10
Mimas
; 11
Methone
; 12
Anthe
; 13
Pallene
; 14
Enceladus
; 15
Tethys
; 15a
Telesto
; 15b
Calypso
; 18
Dione
; 18a
Helene
; 18b
Polydeuces
; 21
Rhea
; 22
Titan
; 23
Hyperion
; 24
Iapetus
; 25
Kiviuq
; 26
Ijiraq
; 27
Phoebe
; 28
Paaliaq
; 29
Skathi
; 30
;S/2004 S 37
; 31
;S/2007 S 2
; 32
Albiorix
; 33
Bebhionn
; 34
;S/2004 S 29
; 35
Erriapus
; 36
;S/2004 S 31
; 37
Skoll
; 38
Siarnaq
; 39
Tarqeq
; 40
;S/2004 S 13
; 41
Hyrrokkin
; 42
Tarvos
; 43
Mundilfari
; 44
;S/2006 S 1
; 45
Greip
; 46
Jarnsaxa
; 47
Bergelmir
; 48
;S/2004 S 17
; 49
Narvi
; 50
;S/2004 S 20
; 51
Suttungr
; 52
Hati
; 53
;S/2004 S 12
; 54
Farbauti
; 55
;S/2004 S 27
; 56
Bestla
; 57
;S/2007 S 3
; 58
Aegir
; 59
;S/2004 S 7
; 60
;S/2004 S 22
; 61
Thrymr
; 62
;S/2004 S 30
; 63
;S/2004 S 23
; 64
;S/2004 S 25
; 65
;S/2004 S 32
; 66
;S/2006 S 3
; 67
;S/2004 S 38
; 68
;S/2004 S 28
; 69
Kari
; 70
;S/2004 S 35
; 71
Fenrir
; 72
;S/2004 S 21
; 73
;S/2004 S 24
; 74
;S/2004 S 36
; 75
Loge
; 76
Surtur
; 77
;S/2004 S 39
; 78
Ymir
; 79
;S/2004 S 33
; 80
;S/2004 S 34
; 81
Fornjot
; 82
;S/2004 S 26

; -- Uranus
; 1
Cordelia
; 2
Ophelia
; 3
Bianca
; 4
Cressida
; 5
Desdemona
; 6
Juliet
; 7
Portia
; 8
Rosalind
; 9
Cupid
; 10
Belinda
; 11
Perdita
; 12
Puck
; 13
Mab
; 14
Miranda
; 15
Ariel
; 16
Umbriel
; 17
Titania
; 18
Oberon
; 19
Francisco
; 20
Caliban
; 21
Stephano
; 22
Trinculo
; 23
Sycorax
; 24
Margaret
; 25
Prospero
; 26
Setebos
; 27
Ferdinand

; -- Neptune
; 1
Naiad
; 2
Thalassa
; 3
Despina
; 4
Galatea
; 5
Larissa
; 6
Hippocamp
; 7
Proteus
; 8
Triton
; 9
Nereid
; 10
Halimede
; 11
Sao
; 12
Laomedeia
; 13
Psamathe
; 14
Neso

; -- Pluto
; 1
Charon
; 2
Styx
; 3
Nix
; 4
Kerberos
; 5
Hydra

; -- Haumea
; 1
Namaka
; 2
;Hi-iaka

; -- Makemake
; 1
;S/2015 (136472) 1

; -- Eris
; 1
;S/2005 (2003 UB 313) 1

; ====
; 元素
; ====

; 1
Hydrogen
; 2
Helium
; 3
Lithium
; 4
Beryllium
; 5
Boron
; 6
Carbon
; 7
Nitrogen
; 8
Oxygen
; 9
Fluorine
; 10
Neon
; 11
Sodium
; 12
Magnesium
; 13
Aluminium
; 14
Silicon
; 15
Phosphorus
; 16
Sulfur
; 17
Chlorine
; 18
Argon
; 19
Potassium
; 20
Calcium
; 21
Scandium
; 22
Titanium
; 23
Vanadium
; 24
Chromium
; 25
Manganese
; 26
Iron
; 27
Cobalt
; 28
Nickel
; 29
Copper
; 30
Zinc
; 31
Gallium
; 32
Germanium
; 33
Arsenic
; 34
Selenium
; 35
Bromine
; 36
Krypton
; 37
Rubidium
; 38
Strontium
; 39
Yttrium
; 40
Zirconium
; 41
Niobium
; 42
Molybdenum
; 43
Technetium
; 44
Ruthenium
; 45
Rhodium
; 46
Palladium
; 47
Silver
; 48
Cadmium
; 49
Indium
; 50
Tin
; 51
Antimony
; 52
Tellurium
; 53
Iodine
; 54
Xenon
; 55
Caesium
; 56
Barium
; 57
Lanthanum
; 58
Cerium
; 59
Praseodymium
; 60
Neodymium
; 61
Promethium
; 62
Samarium
; 63
Europium
; 64
Gadolinium
; 65
Terbium
; 66
Dysprosium
; 67
Holmium
; 68
Erbium
; 69
Thulium
; 70
Ytterbium
; 71
Lutetium
; 72
Hafnium
; 73
Tantalum
; 74
Tungsten
; 75
Rhenium
; 76
Osmium
; 77
Iridium
; 78
Platinum
; 79
Gold
; 80
Mercury
; 81
Thallium
; 82
Lead
; 83
Bismuth
; 84
Polonium
; 85
Astatine
; 86
Radon
; 87
Francium
; 88
Radium
; 89
Actinium
; 90
Thorium
; 91
Protactinium
; 92
Uranium
; 93
Neptunium
; 94
Plutonium
; 95
Americium
; 96
Curium
; 97
Berkelium
; 98
Californium
; 99
Einsteinium
; 100
Fermium
; 101
Mendelevium
; 102
Nobelium
; 103
Lawrencium
; 104
Rutherfordium
; 105
Dubnium
; 106
Seaborgium
; 107
Bohrium
; 108
Hassium
; 109
Meitnerium
; 110
Darmstadtium
; 111
Roentgenium
; 112
Copernicium
; 113
Nihonium
; 114
Flerovium
; 115
Moscovium
; 116
Livermorium
; 117
Tennessine
; 118
Oganesson

; ====
; プリキュア
; ====

; -- All stars
Echo

; -- 無印
Black
White

; -- Max Heart
Luminous

; -- Splush Star
Bloom
Egret
Bright
Windy

; -- 5
Dream
Rouge
Lemonade
Mint
Aqua

; -- 5 GoGo!
Rose

; -- Fresh
Peach
Berry
Pine
Passion

; -- Heart Catch
Blossom
Marine
Sunshine
Moonlight

Flower
;Fire

; -> HUGtto!
;Ange

; -- Suite
Melody
Rhythm
Beat
Muse

; -- Smile
Happy
Sunny
Peace
March
Beauty

; -- Doki^2
Heart
Diamond
Rosetta
Sword
Ace

Empress
Magician
Priestess

;Sebastian
;Cutie Madam

; -- Happiness Charge
Lovely
Princess
Honey
Fortune

Tender

;Bomber Girls
;Wonderful Net

;Merci
Earl

Nile

;Aloha
Sunset
Wave

Continental
Katyusha
;Southern Cross
Gonna
Pantaloni
Matador

;Shelly ???
Sherry

Mirage
;Unlovely

; -- Princess
Flora
Mermaid
Twinkle
Scarlet

; -- 魔法使い
Miracle
Magical
Felice

; -- Kira^2 A La Mode
Whip
Custard
Gelato
Macaron
Chocolat
Parfait

; -- HUGtto!
Yell
Ange
Etoile
Macherie
Amour

Tomorrow

; -- Star Twinkle
Star
Milky
Soleil
Selene
Cosmo

; -- Healin' Good
Grace
Fontaine
Sparkle
Earth

; -- Tropical-Rouge!

Summer
Coral
Papaya
Flamingo

";

		#endregion

		#region 英単語リスト

		public const string 英単語リスト = @"

; --https://progeigo.org/learning/essential-words-600-plus/

; A

abstract			【形容詞】
accept				【動詞】
access				【動詞／名詞】
accessibility		【名詞】
accessible			【形容詞】
account				【名詞】
action				【名詞】
activate			【動詞】
active				【形容詞】
activity			【名詞】
add					【動詞】
additional			【形容詞】
address				【名詞】
adjust				【動詞】
administrator		【名詞】
aggregate			【動詞】
agree				【動詞】
algorithm			【名詞】
alias				【名詞】
allocation			【名詞】
allow				【動詞】
alternative			【形容詞】
annotation			【名詞】
anonymous			【形容詞】
append				【動詞】
applicable			【形容詞】
application			【名詞】
apply				【動詞】
archive				【名詞／動詞】
area				【名詞】
argument			【名詞】
array				【名詞】
assert				【動詞】
asset				【名詞】
assign				【動詞】
assignment			【名詞】
associate			【動詞】
attach				【動詞】
attribute			【名詞】
audio				【名詞】
authentication		【名詞】
author				【名詞】
authorize			【動詞】
automatic			【形容詞】
automatically		【副詞】
availability		【名詞】
available			【形容詞】
avoid				【動詞】

; B

backup				【名詞】
based				【形容詞】
batch				【名詞】
below				【副詞】
binary				【形容詞】
bind				【動詞】
bit					【名詞】
blank				【形容詞】
block				【名詞／動詞】
body				【名詞】
boolean				【形容詞】
boot				【動詞／名詞】
brace				【名詞】
bracket				【名詞】
branch				【名詞】
break				【動詞】
breakpoint			【名詞】
browse				【動詞】
browser				【名詞】
buffer				【名詞／動詞】
bug					【名詞】
build				【動詞／名詞】
bump				【動詞】
bundle				【名詞／動詞】
button				【名詞】
byte				【名詞】

; C

cache				【名詞】
calculate			【動詞】
call				【動詞】
callback			【名詞】
cancel				【動詞】
capability			【名詞】
capacity			【名詞】
capture				【動詞】
caret				【名詞】
case				【名詞】
cast				【動詞】
certificate			【名詞】
change				【動詞】
character			【名詞】
check				【動詞】
checkbox			【名詞】
checkout			【名詞】
child				【名詞】
choice				【名詞】
choose				【動詞】
clarify				【動詞】
class				【名詞】
clean				【形容詞／動詞】
cleanup				【名詞】
clear				【動詞】
click				【動詞】
client				【名詞】
clipboard			【名詞】
clone				【動詞】
close				【動詞】
cloud				【名詞】
cluster				【名詞】
code				【名詞】
collapse			【動詞】
collection			【名詞】
column				【名詞】
command				【名詞】
comment				【名詞／動詞】
commit				【動詞】
communication		【名詞】
compare				【動詞】
compatibility		【名詞】
compile				【動詞】
complete			【動詞／形容詞】
completion			【名詞】
component			【名詞】
compress			【動詞】
compute				【動詞】
condition			【名詞】
conditional			【形容詞】
configuration		【名詞】
configure			【動詞】
confirm				【動詞】
conflict			【動詞／名詞】
connect				【動詞】
connection			【名詞】
console				【名詞】
constant			【名詞／形容詞】
constraint			【名詞】
construct			【動詞】
constructor			【名詞】
contact				【動詞／名詞】
contain				【動詞】
container			【名詞】
content				【名詞】
context				【名詞】
continue			【動詞】
contribute			【動詞】
control				【名詞／動詞】
convert				【動詞】
cookie				【名詞】
coordinate			【名詞】
copy				【動詞／名詞】
copyright			【名詞】
core				【名詞／形容詞】
correct				【形容詞／動詞】
correctly			【副詞】
count				【動詞／名詞】
crash				【動詞／名詞】
create				【動詞】
credential			【名詞】
current				【形容詞】
cursor				【名詞】
custom				【形容詞】
customize			【動詞】

; D

damage				【名詞】
dashboard			【名詞】
data				【名詞】
database			【名詞】
debug				【動詞／名詞】
debugger			【名詞】
decimal				【形容詞】
declaration			【名詞】
declare				【動詞】
decode				【動詞】
dedicated			【形容詞】
default				【名詞／動詞】
define				【動詞】
delay				【名詞】
delete				【動詞】
dependency			【名詞】
deploy				【動詞】
deployment			【名詞】
deprecated			【形容詞】
deprecation			【名詞】
describe			【動詞】
description			【名詞】
descriptor			【名詞】
destination			【名詞】
destroy				【動詞】
detail				【名詞】
detect				【動詞】
determine			【動詞】
developer			【名詞】
development			【名詞】
device				【名詞】
dialog				【名詞】
digit				【名詞】
directory			【名詞】
disable				【動詞】
disk				【名詞】
display				【動詞／名詞】
distribute			【動詞】
document			【名詞／動詞】
documentation		【名詞】
domain				【名詞】
download			【動詞】
driver				【名詞】
;drop-down			【形容詞】
dump				【動詞】
duplicate			【形容詞／動詞】
duration			【名詞】
dynamic				【形容詞】
dynamically			【副詞】

; E

edit				【動詞】
editor				【名詞】
element				【名詞】
email				【名詞／動詞】
emit				【動詞】
empty				【形容詞】
enable				【動詞】
encode				【動詞】
encoding			【名詞】
encounter			【動詞】
encryption			【名詞】
endpoint			【名詞】
ensure				【動詞】
enter				【動詞】
entity				【名詞】
entry				【名詞】
enumeration			【名詞】
environment			【名詞】
equal				【形容詞／動詞】
error				【名詞】
escape				【動詞】
event				【名詞】
example				【名詞】
except				【前置詞】
exception			【名詞】
exclude				【動詞】
executable			【形容詞／名詞】
execute				【動詞】
execution			【名詞】
exist				【動詞】
existing			【形容詞】
exit				【動詞】
expand				【動詞】
expected			【形容詞】
export				【名詞／動詞】
express				【動詞／形容詞】
expression			【名詞】
extend				【動詞】
extension			【名詞】
external			【形容詞】
extra				【形容詞】
extract				【動詞】

; F

fail				【動詞】
failure				【名詞】
fallback			【名詞／動詞】
false				【形容詞】
feature				【名詞】
fetch				【動詞】
field				【名詞】
file				【名詞】
filter				【名詞／動詞】
final				【形容詞】
find				【動詞】
finish				【動詞】
fire				【動詞】
fix					【動詞】
flag				【名詞】
folder				【名詞】
follow				【動詞】
following			【形容詞／名詞】
font				【名詞】
force				【動詞】
form				【名詞】
format				【名詞／動詞】
framework			【名詞】
free				【形容詞】
function			【名詞】
functionality		【名詞】

; G

general				【形容詞】
generate			【動詞】
generation			【名詞】
generic				【形容詞】
get					【動詞】
global				【形容詞】
graphic				【名詞】
group				【名詞】
guide				【名詞】

; H

hack				【名詞／動詞】
handle				【動詞／名詞】
handler				【名詞】
handling			【名詞】
hardware			【名詞】
hash				【名詞】
header				【名詞】
health				【名詞】
height				【名詞】
hide				【動詞】
hierarchy			【名詞】
highlight			【名詞／動詞】
host				【名詞】

; I

icon				【名詞】
identifier			【名詞】
ignore				【動詞】
image				【名詞】
implement			【動詞】
implementation		【名詞】
import				【名詞／動詞】
improve				【動詞】
include				【動詞】
including			【前置詞】
incompatible		【形容詞】
increment			【名詞／動詞】
indentation			【名詞】
index				【名詞】
indicate			【動詞】
information			【名詞】
initial				【形容詞】
initialization		【名詞】
initialize			【動詞】
inline				【形容詞／動詞】
inner				【形容詞】
input				【名詞／動詞】
insert				【動詞】
inspect				【動詞】
inspection			【名詞】
install				【動詞】
installation		【名詞】
installer			【名詞】
instance			【名詞】
instruction			【名詞】
integer				【名詞】
interact			【動詞】
interface			【名詞】
internal			【形容詞】
interval			【名詞】
introduce			【動詞】
invalid				【形容詞】
invoke				【動詞】
issue				【名詞】
item				【名詞】
iterate				【動詞】

; J

join				【動詞】

; K

key					【名詞】

; L

label				【名詞／動詞】
language			【名詞】
latency				【名詞】
later				【副詞】
latest				【形容詞】
launch				【動詞】
layer				【名詞】
layout				【名詞】
length				【名詞】
level				【名詞】
library				【名詞】
license				【名詞／動詞】
limit				【動詞／名詞】
line				【名詞】
link				【名詞／動詞】
list				【名詞／動詞】
listener			【名詞】
load				【動詞】
local				【形容詞】
locale				【名詞】
location			【名詞】
lock				【動詞／名詞】
log					【名詞／動詞】
logic				【名詞】
login				【名詞】
lookup				【名詞】
loop				【名詞】

; M

make				【動詞】
manage				【動詞】
manager				【名詞】
manual				【形容詞／名詞】
manually			【副詞】
map					【名詞／動詞】
master				【名詞】
match				【動詞】
matching			【形容詞】
maximum				【形容詞】
media				【名詞】
memory				【名詞】
menu				【名詞】
merge				【動詞】
message				【名詞】
metadata			【名詞】
method				【名詞】
migration			【名詞】
millisecond			【名詞】
minimum				【形容詞】
minor				【形容詞】
missing				【形容詞】
mock				【形容詞】
mode				【名詞】
model				【名詞】
modification		【名詞】
modifier			【名詞】
modify				【動詞】
module				【名詞】
monitor				【動詞】
move				【動詞】
multiple			【形容詞】

; N

name				【名詞】
namespace			【名詞】
native				【形容詞】
navigate			【動詞】
navigation			【名詞】
nested				【形容詞】
network				【名詞】
next				【形容詞】
node				【名詞】
none				【代名詞】
normal				【形容詞】
normalize			【動詞】
note				【名詞／動詞】
notice				【名詞】
notification		【名詞】
notify				【動詞】
null				【形容詞／名詞】
number				【名詞】
numeric				【形容詞】

; O

object				【名詞】
obtain				【動詞】
occur				【動詞】
occurrence			【名詞】
offset				【名詞】
open				【動詞／形容詞】
operation			【名詞】
optimize			【動詞】
option				【名詞】
optional			【形容詞】
otherwise			【副詞】
output				【名詞／動詞】
override			【動詞】
overview			【名詞】
overwrite			【動詞】
owner				【名詞】

; P

package				【名詞】
padding				【名詞】
pane				【名詞】
parameter			【名詞】
parent				【名詞】
parenthesis			【名詞】
parse				【動詞】
pass				【動詞】
password			【名詞】
patch				【名詞】
path				【名詞】
payload				【名詞】
perform				【動詞】
performance			【名詞】
permission			【名詞】
permit				【動詞】
persistence			【名詞】
physical			【形容詞】
placeholder			【名詞】
play				【動詞】
plugin				【名詞】
pool				【名詞】
populate			【動詞】
popup				【名詞】
port				【名詞】
position			【名詞】
post				【動詞／名詞】
preference			【名詞】
preferred			【形容詞】
prefix				【名詞】
prepare				【動詞】
press				【動詞】
preview				【名詞／動詞】
previous			【形容詞】
print				【動詞】
priority			【名詞】
private				【形容詞】
problem				【名詞】
process				【名詞／動詞】
product				【名詞】
profile				【名詞】
progress			【名詞】
properly			【副詞】
property			【名詞】
protect				【動詞】
protocol			【名詞】
prototype			【名詞】
provide				【動詞】
provider			【名詞】
provision			【名詞／動詞】
proxy				【名詞】
public				【形容詞】
publish				【動詞】
pull				【動詞／名詞】
push				【動詞】

; Q

query				【名詞】
queue				【名詞】

; R

range				【名詞】
raw					【形容詞】
read				【動詞】
;real-time			【形容詞】
receive				【動詞】
recommend			【動詞】
record				【動詞／名詞】
redirect			【動詞】
redundant			【形容詞】
refactor			【動詞】
refer				【動詞】
reference			【名詞／動詞】
refresh				【動詞】
register			【動詞】
registration		【名詞】
registry			【名詞】
release				【動詞】
reload				【動詞】
remote				【形容詞】
removal				【名詞】
remove				【動詞】
rename				【動詞】
render				【動詞】
replace				【動詞】
report				【動詞／名詞】
repository			【名詞】
represent			【動詞】
representation		【名詞】
request				【名詞／動詞】
require				【動詞】
reserve				【動詞】
reset				【動詞】
resize				【動詞】
resolution			【名詞】
resolve				【動詞】
resource			【名詞】
respond				【動詞】
response			【名詞】
restart				【動詞】
restore				【動詞】
restriction			【名詞】
result				【名詞】
retrieve			【動詞】
return				【動詞】
revert				【動詞】
revision			【名詞】
;right-click		【動詞】
role				【名詞】
root				【名詞】
route				【名詞】
row					【名詞】
rule				【名詞】
run					【動詞】
runtime				【名詞】

; S

sanitize			【動詞】
save				【動詞】
scale				【動詞】
scan				【動詞】
schedule			【名詞／動詞】
schema				【名詞】
scope				【名詞】
screen				【名詞】
script				【名詞】
scroll				【動詞】
search				【動詞／名詞】
section				【名詞】
secure				【形容詞】
security			【名詞】
see					【動詞】
select				【動詞】
send				【動詞】
separator			【名詞】
sequence			【名詞】
serialize			【動詞】
server				【名詞】
service				【名詞】
session				【名詞】
set					【動詞】
setting				【名詞】
setup				【名詞】
shape				【名詞】
share				【動詞】
shortcut			【名詞】
show				【動詞】
shutdown			【名詞】
sign				【名詞／動詞】
signature			【名詞】
simplify			【動詞】
size				【名詞】
skip				【動詞】
snapshot			【名詞】
socket				【名詞】
software			【名詞】
sort				【動詞】
source				【名詞】
space				【名詞】
specific			【形容詞】
specification		【名詞】
specify				【動詞】
split				【動詞】
stack				【名詞】
standalone			【形容詞】
start				【動詞】
startup				【名詞】
state				【名詞】
statement			【名詞】
static				【形容詞】
status				【名詞】
step				【名詞】
stop				【動詞】
storage				【名詞】
store				【動詞】
stream				【名詞】
string				【名詞】
style				【名詞】
submit				【動詞】
successful			【形容詞】
successfully		【副詞】
suffix				【名詞】
supply				【動詞】
support				【動詞／名詞】
suppress			【動詞】
switch				【動詞／名詞】
synchronize			【動詞】
syntax				【名詞】
system				【名詞】

; T

tab					【名詞】
table				【名詞】
tag					【名詞】
target				【名詞】
task				【名詞】
template			【名詞】
temporary			【形容詞】
terminate			【動詞】
termination			【名詞】
terms				【名詞】
test				【名詞／動詞】
thirdparty			【名詞】
thread				【名詞】
throw				【動詞】
timeout				【名詞】
timestamp			【名詞】
todo				【名詞】
toggle				【動詞】
token				【名詞】
touch				【名詞／動詞】
traffic				【名詞】
transaction			【名詞】
transfer			【動詞】
transform			【動詞】
transition			【名詞】
translate			【動詞】
tree				【名詞】
trigger				【動詞】
true				【形容詞】
tweak				【動詞】
type				【名詞／動詞】
typo				【名詞】

; U

unable				【形容詞】
unauthorized		【形容詞】
unavailable			【形容詞】
undefined			【形容詞】
undo				【動詞】
unexpected			【形容詞】
uninstall			【動詞】
unique				【形容詞】
unit				【名詞】
unknown				【形容詞】
unnecessary			【形容詞】
unresolved			【形容詞】
unsupported			【形容詞】
unused				【形容詞】
update				【動詞／名詞】
upgrade				【動詞】
upload				【動詞／名詞】
usage				【名詞】
use					【動詞／名詞】
useful				【形容詞】
user				【名詞】
username			【名詞】

; V

valid				【形容詞】
validate			【動詞】
validation			【名詞】
value				【名詞】
variable			【名詞】
verbose				【形容詞】
verify				【動詞】
version				【名詞】
view				【動詞／名詞】
virtual				【形容詞】
visibility			【名詞】
visible				【形容詞】
visit				【動詞】

; W

wait				【動詞】
warn				【動詞】
warning				【名詞】
website				【名詞】
whitespace			【名詞】
widget				【名詞】
width				【名詞】
window				【名詞】
wizard				【名詞】
wrap				【動詞】
wrapper				【名詞】
write				【動詞】

; Y

you					【代名詞】
your				【代名詞】

; Z

zip					【動詞】

";

		#endregion

		#region クラス用ダミーメンバー

		/// <summary>
		/// 要置き換え : SSS_ to (RANDOM_WORD)_
		/// </summary>
		public static string CLASS_DUMMY_MEMBER = @"

		public static int SSS_Count;

		public int SSS_GetCount()
		{
			return SSS_Count;
		}

		public void SSS_SetCount(int SSS_SetCount_Prm)
		{
			SSS_Count = SSS_SetCount_Prm;
		}

		public void SSS_ResetCount()
		{
			this.SSS_SetCount(0);
		}

		public int SSS_NextCount()
		{
			return SSS_Count++;
		}

		public class SSS_ValueInfo
		{
			public int SSS_ValueInfo_A;
			public int SSS_ValueInfo_B;
			public int SSS_ValueInfo_C;
		}

		public static SSS_ValueInfo SSS_Value;

		public SSS_ValueInfo SSS_GetValue()
		{
			return SSS_Value;
		}

		public void SSS_SetValue(SSS_ValueInfo SSS_SetValue_Prm)
		{
			SSS_Value = SSS_SetValue_Prm;
		}

		public void SSS_Overload_00()
		{
			this.SSS_Overload_01(this.SSS_NextCount());
		}

		public void SSS_Overload_01(int SSS_a)
		{
			this.SSS_Overload_02(SSS_a, this.SSS_NextCount());
		}

		public void SSS_Overload_02(int SSS_a, int SSS_b)
		{
			this.SSS_Overload_03(SSS_a, SSS_b, this.SSS_NextCount());
		}

		public void SSS_Overload_03(int SSS_a, int SSS_b, int SSS_c)
		{
			this.SSS_Overload_04(SSS_a, SSS_b, SSS_c, this.SSS_GetValue().SSS_ValueInfo_A, this.SSS_GetValue().SSS_ValueInfo_B, this.SSS_GetValue().SSS_ValueInfo_C);
		}

		public void SSS_Overload_04(int SSS_a, int SSS_b, int SSS_c, int SSS_a2, int SSS_b2, int SSS_c2)
		{
			var SSS_infos = new[]
			{
				new { SSS_Info_P1 = SSS_a, SSS_Info_P2 = SSS_a2 },
				new { SSS_Info_P1 = SSS_b, SSS_Info_P2 = SSS_a2 },
				new { SSS_Info_P1 = SSS_c, SSS_Info_P2 = SSS_a2 },
			};

			this.SSS_SetValue(new SSS_ValueInfo()
			{
				SSS_ValueInfo_A = SSS_a,
				SSS_ValueInfo_B = SSS_b,
				SSS_ValueInfo_C = SSS_c,
			});

			if (SSS_infos[0].SSS_Info_P1 == SSS_a2) this.SSS_Overload_05(SSS_infos[0].SSS_Info_P2);
			if (SSS_infos[1].SSS_Info_P1 == SSS_b2) this.SSS_Overload_05(SSS_infos[1].SSS_Info_P2);
			if (SSS_infos[2].SSS_Info_P1 == SSS_c2) this.SSS_Overload_05(SSS_infos[2].SSS_Info_P2);
		}

		public void SSS_Overload_05(int SSS_v)
		{
			if (SSS_v != this.SSS_GetCount())
				this.SSS_SetCount(SSS_v);
			else
				this.SSS_Overload_01(SSS_v);
		}

";

		#endregion

		#region 構造体用ダミーメンバー

		/// <summary>
		/// 要置き換え : SSS_ to (RANDOM_WORD)_
		/// </summary>
		public static string STRUCT_DUMMY_MEMBER = @"

		public void SSS_Overload_00()
		{
			this.SSS_Overload_01(this.SSS_NextCount());
		}

		public void SSS_Overload_01(int SSS_a)
		{
			this.SSS_Overload_02(SSS_a, this.SSS_NextCount());
		}

		public void SSS_Overload_02(int SSS_a, int SSS_b)
		{
			this.SSS_Overload_03(SSS_a, SSS_b, this.SSS_NextCount());
		}

		public void SSS_Overload_03(int SSS_a, int SSS_b, int SSS_c)
		{
			this.SSS_Overload_04(SSS_a, SSS_b, SSS_c, this.SSS_NextCount());
		}

		public void SSS_Overload_04(int SSS_a, int SSS_b, int SSS_c, int SSS_d)
		{
			var SSS_infos = new[]
			{
				new { SSS_Info_P1 = SSS_a, SSS_Info_P2 = SSS_b },
				new { SSS_Info_P1 = SSS_c, SSS_Info_P2 = SSS_d },
			};

			this.SSS_AddToCount(SSS_a);
			this.SSS_AddToCount(SSS_b);
			this.SSS_AddToCount(SSS_c);
			this.SSS_AddToCount(SSS_d);

			if (SSS_infos[0].SSS_Info_P1 == this.SSS_NextCount()) this.SSS_AddToCount_02(SSS_infos[0].SSS_Info_P2);
			if (SSS_infos[1].SSS_Info_P1 == this.SSS_NextCount()) this.SSS_AddToCount_02(SSS_infos[1].SSS_Info_P2);
		}

		public static int SSS_Count;

		public int SSS_NextCount()
		{
			return SSS_Count++;
		}

		public void SSS_AddToCount(int SSS_valueForAdd)
		{
			SSS_Count += SSS_valueForAdd;
		}

		public void SSS_AddToCount_02(int SSS_valueForAdd_02)
		{
			SSS_Count -= SSS_valueForAdd_02;
			SSS_Overload_00();
		}

";

		#endregion
	}
}
