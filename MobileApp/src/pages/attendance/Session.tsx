import { useState } from 'react';
import { View, Text, Platform, ScrollView, ActivityIndicator } from 'react-native';
import attendanceStyle from '../../styles/attendanceStyle';
import WebQRScanner from '../../components/QRCodeScanner.web';
import NativeQRScanner from '../../components/QRCodeScanner.native';
import colors from '../../enums/colors';
import { SessionService } from '../../services/SessionService';
import { parseSessionQRCode } from '../../utils/uri';

export default function SessionAttendance() {
  const [qrResult, setQrResult] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [loading, setLoading] = useState(false);

  const handleScan = async (result: string) => {
    if (!result) return;
    
    if (!result.startsWith('io-event-connecter://')) {
      setError('Invalid QR code');
      return;
    }

    setLoading(true);

    try {
      const parsedResult = parseSessionQRCode(result);
      if (!parsedResult) {
        setError('Invalid QR code format');
        return;
      }

      const data = await SessionService.markAttendance(parsedResult.sessionId, parsedResult.userId);
      if (!data.success) {
        setError(data.message);
        return;
      }
      
      setQrResult(result || '');
      setError('');
    } catch (err) {
      setError('Failed to process QR code. Please try again.');
    } finally {
      setTimeout(() => {
        setLoading(false);
      }, 1000); // Simulate processing time
    }
  }

  if(loading) {
    return (
      <View style={attendanceStyle.centered}>
        <ActivityIndicator size="large" color={colors.DEFAULT_BUTTON_COLOR} />
        <Text style={attendanceStyle.hint}>Processing attendance...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={attendanceStyle.container}> 
      {Platform.OS === 'web' ? (
        <WebQRScanner onScan={handleScan} />
      ) : (
        <NativeQRScanner onScan={handleScan} />
      )}

      <View style={{ width: '100%', height: 2, backgroundColor: '#b1b1b1', marginVertical: 16 }} />

      <View style={{ backgroundColor: '#f0f0f0' }}>
        <View style={{ }}>
          {error ? 
            <ErrorMessage error={error} /> 
          : qrResult ? 
            <SuccessMessage />
          : 
            <Text style={{ textAlign: 'center', paddingVertical: 16, fontSize: 16 }}>No QR code scanned yet</Text>
          }
        </View>
      </View>
    </ScrollView>
  )
};

function SuccessMessage() {
  return (
    <View style={{ alignItems: 'center', justifyContent: 'center', display: 'flex', paddingVertical: 16 }}>
      <Text style={{ textAlign: 'center', paddingBottom: 6, color: 'green', fontSize: 48 }}>✔</Text>
      <Text style={{ textAlign: 'center', color: 'green', fontSize: 16, fontWeight: 'bold' }}>Successfully added Attendance!</Text>
    </View>
  )
}

function ErrorMessage({ error }: { error: string }) {
  return (
    <View style={{ alignItems: 'center', justifyContent: 'center', display: 'flex', paddingVertical: 16 }}>
      <Text style={{ textAlign: 'center', paddingBottom: 12, fontSize: 48 }}>❌</Text>
      <Text style={{ textAlign: 'center', color: 'red', fontSize: 16, fontWeight: 'bold' }}>Error: {error}</Text>
    </View>
  )
}