   M S B A M L   `   `   `  ����@  <Banka, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Banka.FormeBanka.Forme  5   	       Banka.Forme.frmKredit   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Banka.Forme     5   	     � Title$  Dodaj/Izmeni kredit��5   	   $	��300��6%   $	��330��62   
����ح� � Icon$? 9C:\Users\Pheob\OneDrive\Desktop\PIS-BANKA\Banka\banka.png��5	       
������� � WindowStartupLocation$ CenterScreen=�5
   	   .��5      � .+�5   
   �� $��Iznos��6   $
��Left=�6    $��	30,20,0,0q�6;   $	��Top=�6N   �� 5   
   $��Rata:��6   $
��Left=�6    $��	30,50,0,0q�6;   $	��Top=�6N   �� 5   
   $��Kamata:��6   $
��Left=�6"   $��	30,80,0,0q�6=   $	��Top=�6P   �� 5   
   $��Pocetak otplacivanja:��6   $
��Left=�60   $��
30,110,0,0q�6K   $	��Top=�6_   �� 5   
   $��Zavrsetak otplacivanja:��6   $
��Left=�62   $��
30,140,0,0q�6M   $	��Top=�6a   �� 5   
   $��Racun:��6   $
��Left=�6!   $��
30,170,0,0q�6<   $	��Top=�6P   �� 5   
   -   5         ��Name$ txtIznos��5      $��
170,20,0,0q�6$   $
��Left=�68   $	��Top=�6U   $��23��6m   $	��120��6y    �� TextWrapping$
 Wrap=�6�   �� 5   
   -   5        $ txtRata��5      $��
170,50,0,0q�6#   $
��Left=�67   $	��Top=�6T   $��23��6l   $	��120��6x   $
 Wrap=�6�   �� 5   
   -   5        $ 	txtKamata��5      $��
170,80,0,0q�6%   $
��Left=�69   $	��Top=�6V   $��23��6n   $	��120��6z   $
 Wrap=�6�   (  "System.Windows.Controls.DatePicker  5   
   -   5          Name$ dpPocetakOtplacivanja��5      $
��Left=�64   $��170,110,0,0q�6O   $	��Top=�6d   $��23��6|   $	��120��6�     5   
   -   5        $ dpZavrsetakOtplacivanja��5      $
��Left=�66   $��170,140,0,0q�6Q   $	��Top=�6f   $��23��6~   $	��120��6�   �� 5   
   -   5         ��Name$ 	cbRacunID��5      $
��Left=�6&   $��170,170,0,0q�6A   $	��Top=�6V   $��23��6n   $	��120��6z    �� DisplayMemberPath$ 
BrojRacuna��6�    �� SelectedValuePath$ RacunID��6�   �� 5   
   -   5        	 ��Name$	 
btnSacuvaj��5      $��Sacuvaj��6&   $
��Left=�68   $��
73,229,0,0q�6S   $	��Top=�6g   6   �� 5   
   -   5        $	 	btnOtkazi��5      $��Otkazi��6$   $
��Left=�65   $��230,229,0,0q�6P   $	��Top=�6e   
 J� RenderTransformOrigin$
 -1.802,0.468&�6}   6�   5      5      