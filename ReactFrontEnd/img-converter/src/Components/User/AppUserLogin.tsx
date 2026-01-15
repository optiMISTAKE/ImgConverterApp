import React, { useState } from 'react';
import { 
  TextInput, PasswordInput, Button, Paper, Title, Container, Group, Anchor, Alert, Stack, Text 
} from '@mantine/core';
import { IconAlertCircle } from '@tabler/icons-react';
import { useNavigate, useLocation, Link } from 'react-router-dom';
import { useAuth } from './AuthContext';

export default function AppUserLogin() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      // Default to home ("/") if no specific return url exists
      const from = (location.state as any)?.from?.pathname || '/';
      await login(email, password);
      navigate(from, { replace: true });
    } catch (err: any) {
      if (err?.response?.status === 401 || err?.response?.status === 400) {
        setError('Invalid email or password.');
      } else {
        setError('Login failed. Please try again later.');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container size={420} my={40}>
      <Title ta="center" order={2}>
        Welcome back!
      </Title>
      <Text c="dimmed" size="sm" ta="center" mt={5}>
        Do not have an account yet?{' '}
        <Anchor component={Link} to="/account/register" size="sm">
          Create account
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
            
            <PasswordInput 
              label="Password" 
              placeholder="Your password" 
              required 
              mt="md"
              value={password}
              onChange={(e) => setPassword(e.currentTarget.value)}
            />

            <Button fullWidth mt="xl" type="submit" loading={loading}>
              Sign in
            </Button>
          </Stack>
        </form>
      </Paper>
    </Container>
  );
}