   M S B A M L   `   `   `  ����  MechanotronicsApp"    MechanotronicsApp.MainWindow   5   	   W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$:  4Механотроника - Главное окно��5   	   $	��550��6.   $
��1000��6;   .��5      � $��10q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5	      $
��Auto �6   .+�5   
   �� $��0��6    �� Orientation$ 
Horizontal=�6"   $��0,0,0,10q�6;   .+�5      �� -   5         ��Name$ StartCarButton��5      $B��<Запустить генерацию автомобилей��6-   $��0,0,10,0q�6W   $��5q�6i   6u   �� 5      -   5        $ StopCarButton��5      $D��>Остановить генерацию автомобилей��6,   $��0,0,10,0q�6W   $��5q�6i   6u   �� 5      -   5        $ StartDriverButton��5      $>��8Запустить генерацию водителей��60   $��0,0,10,0q�6X   $��5q�6j   6v   �� 5      -   5        $ StopDriverButton��5      $@��:Остановить генерацию водителей��6/   $��0,0,10,0q�6X   $��5q�6j   6v   �� 5      -   5        $ OpenDetailsButton��5      $!��Открыть детали��60   $��5q�6I   6U   5      &   System.Windows.Controls.DataGrid  5   
   -   5          Name$ MainDataGrid��5      $��1��6)      AutoGenerateColumns$ False��66      
IsReadOnly$
 True��6R      Columns 5      0  *System.Windows.Controls.DataGridTextColumn  5      ,  &System.Windows.Controls.DataGridColumn   Header$ 
Время��6%   	   Width5  /System.Windows.Controls.DataGridLengthConverter$		 150 6R   1  +System.Windows.Controls.DataGridBoundColumn
   Binding
 64   �� *	Timestamp+6R     5      $ Автомобиль��6%   $	 * 6U   
 69   �� *	CarName+6U     5      $ Водитель��6%   $	 * 6V   
 67   �� *
DriverName+6V   5      �� 5   
   $��2��6   .@�5      �� .��5      �� -   5         ��Name$ 
StatusText��5      $��Готово��60   5      5       5!      5"      