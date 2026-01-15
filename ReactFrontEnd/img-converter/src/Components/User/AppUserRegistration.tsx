import React, { useState } from 'react';
import { 
  TextInput, PasswordInput, Button, Paper, Title, Container, Anchor, Alert, Stack, Text 
} from '@mantine/core';
import { IconAlertCircle } from '@tabler/icons-react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { useAuth } from './AuthContext';

export default function AppUserRegistration() {
  const { register } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [email, setEmail] = useState('');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const validatePassword = (pw: string): string | null => {
    if (pw.length < 6) return 'Password must be at least 6 characters long.';
    if (!/[A-Z]/.test(pw)) return 'Password must contain at least one uppercase letter.';
    if (!/[a-z]/.test(pw)) return 'Password must contain at least one lowercase letter.';
    if (!/[0-9]/.test(pw)) return 'Password must contain at least one digit.';
    return null;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    // Client-side validation
    const pwErr = validatePassword(password);
    if (pwErr) {
        setError(pwErr);
        setLoading(false);
        return;
    }

    if (password !== confirmPassword) {
        setError('Passwords do not match.');
        setLoading(false);
        return;
    }

    try {
        const from = (location.state as any)?.from?.pathname || '/';
        await register(email, username, password);
        navigate(from, { replace: true });
    } catch (err: any) {
        if (err?.response?.status === 400) {
            // Identity errors usually come back as an array description in your backend logic
            // You might need to adjust this depending on exactly how your backend formats the error
            const backendMsg = err.response.data.error || 'Registration failed. Check your details.';
            setError(backendMsg);
        } else {
            setError('Registration failed. Please try again later.');
        }
    } finally {
        setLoading(false);
    }
  };

  return (
    <Container size={420} my={40}>
      <Title ta="center" order={2}>
        Create an Account
      </Title>
      <Text c="dimmed" size="sm" ta="center" mt={5}>
        Already have an account?{' '}
        <Anchor component={Link} to="/account/login" size="sm">
          Login
        </Anchor>
      </Text>

      <Paper withBorder shadow="md" p={30} mt={30} radius="md">
        <form onSubmit={handleSubmit}>
          <Stack>
            {error && (
              <Alert icon={<IconAlertCircle size={16} />} title="Error" color="red" variant="light">
                {error}
              </Alert>
            )}

            <TextInput 
              label="Email" 
              placeholder="you@example.com" 
              required 
              value={email}
              onChange={(e) => setEmail(e.currentTarget.value)}
            />

            <TextInput 
              label="Username" 
              placeholder="Choose a username" 
              required 
              mt="md"
              value={username}
              onChange={(e) => setUsername(e.currentTarget.value)}
            />
            
            <PasswordInput 
              label="Password" 
              placeholder="Your password" 
              description="Must include uppercase, lowercase, and digit"
              required 
              mt="md"
              value={password}
              onChange={(e) => setPassword(e.currentTarget.value)}
            />

            <PasswordInput 
              label="Confirm Password" 
              placeholder="Confirm password" 
              required 
              mt="md"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.currentTarget.value)}
            />

            <Button fullWidth mt="xl" type="submit" loading={loading}>
              Register
            </Button>
          </Stack>
        </form>
      </Paper>
    </Container>
  );
}