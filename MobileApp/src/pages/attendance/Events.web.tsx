import { useState } from 'react';
import { View, Text } from 'react-native';
import { qrCodeStyles } from '../../styles/attendanceStyle';
import QRScanner from '../../components/QRCodeScanner';

export default function EventsAttendance() {
  const [qrResult, setQrResult] = useState<string>('');

  const handleScan = (result: any) => {
    if (!result) return;
    
    setQrResult(result || '');
  }

  return (
    <View style={qrCodeStyles.container}>
      <View>
        <Text style={{ textAlign: 'center', paddingVertical: 16, fontSize: 18, fontWeight: '600' }}>
          Attendance QR Code Scanner
        </Text>
      </View>

      <View style={{ width: '100%', height: 2, backgroundColor: '#b1b1b1', marginVertical: 16 }} />

      <QRScanner onScan={handleScan} />

      <View style={{ width: '100%', height: 2, backgroundColor: '#b1b1b1', marginVertical: 16 }} />

      <View>
        <Text style={{ textAlign: 'center', paddingVertical: 16 }}>{qrResult || 'No QR code scanned yet.'}</Text>
      </View>
    </View>
  )
};