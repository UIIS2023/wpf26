   M S B A M L   `   `   `  ����@  <Banka, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:Banka.FormeBanka.Forme  5   	       Banka.Forme.frmRacun   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:Banka.Forme     5   	     � Title$  Dodaj/Izmeni racun��5   	   $	��260��6$   $	��330��61   
����ح� � Icon$? 9C:\Users\Pheob\OneDrive\Desktop\PIS-BANKA\Banka\banka.png��5	       
������� � WindowStartupLocation$ CenterScreen=�5
   	   .��5      � .+�5   
   �� $��Stanje na racunu:��6   $
��Left=�6,   $��	30,20,0,0q�6G   $	��Top=�6Z   �� 5   
   $��Broj racuna:��6   $
��Left=�6'   $��	30,50,0,0q�6B   $	��Top=�6U   �� 5   
   $��Tip racuna:��6   $
��Left=�6&   $��	30,80,0,0q�6A   $	��Top=�6T   �� 5   
   $��Klijent:��6   $
��Left=�6#   $��
30,110,0,0q�6>   $	��Top=�6R   �� 5   
   -   5         ��Name$ txtStanjeNaRacunu��5      $��
170,20,0,0q�6-   $
��Left=�6A   $	��Top=�6^   $��23��6v   $	��120��6�    �� TextWrapping$
 Wrap=�6�   �� 5   
   -   5        $ txtBrojRacuna��5      $��
170,50,0,0q�6)   $
��Left=�6=   $	��Top=�6Z   $��23��6r   $	��120��6~   $
 Wrap=�6�   �� 5   
   -   5         ��Name$ cbTipRacunaID��5      $��
170,80,0,0q�6*   $
��Left=�6>   $	��Top=�6[   $��23��6s   $	��120��6    �� DisplayMemberPath$ NazivTipaRacuna��6�    �� SelectedValuePath$ TipRacunaID��6�   �� 5   
   -   5        $ cbKlijentID��5      $��170,110,0,0q�6(   $
��Left=�6=   $	��Top=�6Z   $��23��6r   $	��120��6~   $ Klijent��6�   $ 	KlijentID��6�   �� 5   
   -   5         ��Name$ 
btnSacuvaj��5      $��Sacuvaj��6&   $
��Left=�68   $��
64,174,0,0q�6S   $	��Top=�6g   6   �� 5   
   -   5        $ 	btnOtkazi��5      $��Otkazi��6$   $
��Left=�65   $��230,174,0,0q�6P   $	��Top=�6e   6}   5      5      