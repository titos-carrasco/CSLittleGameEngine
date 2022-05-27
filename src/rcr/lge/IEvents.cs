namespace rcr
{
    namespace lge
    {
        /**
        * Interfaz para implementar el evento onMainUpdate y que se ejecuta justo
        * despues de invocar al evento onUpdate() de los GameObjects
        *
        * @author Roberto carrasco (titos.carrasco@gmail.com)
        *
        */
        public interface IEvents
        {
            /**
            * Manjeador del evento onMainUpdate
            *
            * @param dt tiempo en segundos desde el ultimo ciclo del main loop
            */
            public void OnMainUpdate(float dt);
        }
    }
}
